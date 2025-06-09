using Docker.DotNet;
using Docker.DotNet.Models;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Configuration;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text.Json;
using YamlDotNet.Serialization;

namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.Orchestration;

public class ObservabilityClient(
    DockerClient client,
    ILogger<ObservabilityClient> logger,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    IOptions<AppConfiguration> appConfiguration) : IObservabilityClient
{
    private readonly string _prometheusPassword = configuration["Keys:Prometheus"] ??
                                                  throw new ArgumentNullException(
                                                      "Prometheus password is not set in the configuration");

    private readonly bool InDocker = appConfiguration.Value.InDocker;

    private const string PrometheusImageName = "quay.io/prometheus/prometheus";
    private const string LokiImageName = "grafana/loki";
    private const string GrafanaImageName = "grafana/grafana";
    private const string CadvisorImageName = "gcr.io/cadvisor/cadvisor";

    internal class PrometheusGlobalConfig
    {
        public string scrape_interval { get; set; }
    }

    internal class StaticConfig
    {
        public string[] targets { get; set; }
    }

    internal class ScrapeConfig
    {
        public string job_name { get; set; }
        public StaticConfig[] static_configs { get; set; }
    }

    internal class PrometheusConfig
    {
        public PrometheusGlobalConfig global { get; set; }
        public ScrapeConfig[]? scrape_configs { get; set; }
    }

    public async Task EnsureCreated()
    {
        await RestartOrCreateCadvisor();
        await RestartOrCreatePrometheus();
        await RestartOrCreateLoki();
    }

    public async Task CreateOrRunGrafana()
    {
        var network = await client.Networks.InspectNetworkAsync(DockerNamingHelper.ObservabilityNetworkName);
        if (network == null)
        {
            logger.LogInformation("Observability network does not exist. Creating...");
            throw new Exception("Observability network does not exist. Creating...");
        }

        await client.Images.CreateImageAsync(
            new ImagesCreateParameters { FromImage = GrafanaImageName, Tag = "latest" },
            null,
            new Progress<JSONMessage>());
        try
        {
            var container = await client.Containers.InspectContainerAsync(DockerNamingHelper.GrafanaContainerName);
            if (container.State.Running)
            {
                await client.Containers.RestartContainerAsync(DockerNamingHelper.GrafanaContainerName,
                    new ContainerRestartParameters());
                logger.LogInformation("Grafana container is running, Restarting.");
                return;
            }

            await client.Containers.StartContainerAsync(DockerNamingHelper.GrafanaContainerName,
                new ContainerStartParameters());
            logger.LogInformation("Grafana container is not running. Starting...");
            return;
        }
        catch (Exception ex)
        {
            logger.LogInformation(ex, "Grafana container does not exist. Creating...");
        }

        try
        {
            await client.Volumes.CreateAsync(
                new VolumesCreateParameters { Name = DockerNamingHelper.GrafanaVolumeName });
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to create Grafana volume. It may already exist.");
        }

        var createContainerParameters = new CreateContainerParameters
        {
            Name = DockerNamingHelper.GrafanaContainerName,
            Image = $"{GrafanaImageName}:latest",
            Env = new List<string>
            {
                "GF_PATHS_PROVISIONING=/etc/grafana/provisioning",
                "GF_FEATURE_TOGGLES_ENABLE=alertingSimplifiedRouting,alertingQueryAndExpressionsStepMode",
                "PROMETHEUS_DATASOURCE_PASSWORD=" + _prometheusPassword,
            },
            HostConfig = new HostConfig
            {
                RestartPolicy = new RestartPolicy { Name = RestartPolicyKind.Always, MaximumRetryCount = 0 },
                Binds = new List<string>
                {
                    $"{DockerNamingHelper.GrafanaVolumeName}:/var/lib/grafana",
                    $"{DockerLocalStorageHelper.CopyAndResolvePersistedPath(GetGrafanaPrometheusConfigYml())}:/etc/grafana/provisioning/datasources/prometheus.yml",
                    $"{DockerLocalStorageHelper.CopyAndResolvePersistedPath(GetGrafanaLokiConfigYml())}:/etc/grafana/provisioning/datasources/loki.yml",
                },
                PortBindings = InDocker
                    ? new Dictionary<string, IList<PortBinding>>()
                    : new Dictionary<string, IList<PortBinding>>
                    {
                        { "3000/tcp", new List<PortBinding> { new() { HostPort = "3000" } } }
                    }
            },
            ExposedPorts = new Dictionary<string, EmptyStruct> { { "3000/tcp", default } },
            NetworkingConfig = new NetworkingConfig
            {
                EndpointsConfig = new Dictionary<string, EndpointSettings>
                {
                    { DockerNamingHelper.ObservabilityNetworkName, new EndpointSettings() }
                }
            },
        };
        var containerCreated = await client.Containers.CreateContainerAsync(createContainerParameters);
        await client.Containers.StartContainerAsync(containerCreated.ID, new ContainerStartParameters());
        logger.LogInformation("Grafana container created and started");
    }

    private string GetGrafanaLokiConfigYml()
    {
        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var yamlToEdit = Path.Combine(
            currentDirectory,
            "Containerization/Clients/Orchestration/FileTemplates/grafana_provision_loki.yml");
        return yamlToEdit;
    }

    private string GetGrafanaPrometheusConfigYml()
    {
        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var yamlToEdit = Path.Combine(
            currentDirectory,
            "Containerization/Clients/Orchestration/FileTemplates/grafana_provision_prometheus.yml");
        return yamlToEdit;
    }

    public async Task AttachCollector(Guid id, string jobName)
    {
        await EnsureCreated();
        var containerName = DockerNamingHelper.GetSidecarTelemetryName(id);
        var url = $"{containerName}:8889";
        var yamlToEdit = GetPrometheusConfigYaml();
        var deserializer = new DeserializerBuilder().Build();
        var serializer = new SerializerBuilder().Build();
        string yaml = string.Empty;
        using (var reader = new StreamReader(yamlToEdit))
        {
            yaml = await reader.ReadToEndAsync();
        }

        var deserializedYaml = deserializer.Deserialize<PrometheusConfig>(yaml);
        var scrapeConfig = new ScrapeConfig
        {
            job_name = jobName, static_configs = [new StaticConfig { targets = [url] }]
        };
        deserializedYaml.scrape_configs = (deserializedYaml.scrape_configs ?? []).Append(scrapeConfig).ToArray();
        var serializedYaml = serializer.Serialize(deserializedYaml);
        await File.WriteAllTextAsync(yamlToEdit, serializedYaml);
        await RestartOrCreatePrometheus();
        await RestartOrCreateLoki();
    }

    public async Task<PrometheusResultDto?> QueryPrometheusRange(
        string query,
        DateTime requestStart,
        DateTime requestEnd,
        string? requestStep,
        string? requestTimeout,
        int? requestLimit)
        => await QueryRange(
            query,
            requestStart,
            requestEnd,
            requestStep,
            requestTimeout,
            requestLimit,
            "Prometheus");

    public async Task<PrometheusResultDto?> QueryLokiRange(
        string query,
        DateTime requestStart,
        DateTime requestEnd,
        string? requestStep,
        string? requestTimeout,
        int? requestLimit)
        => await QueryRange(
            query,
            requestStart,
            requestEnd,
            requestStep,
            requestTimeout,
            requestLimit,
            "Loki");

    public async Task<PrometheusResultDto?> QueryRange(
        string query,
        DateTime requestStart,
        DateTime requestEnd,
        string? requestStep,
        string? requestTimeout,
        int? requestLimit,
        string httpClientId)
    {
        var httpClient = httpClientFactory.CreateClient(httpClientId);
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"admin:{_prometheusPassword}")));
        var url = $"api/v1/query_range?query={Uri.EscapeDataString(query)}&start={requestStart:O}&end={requestEnd:O}";
        if (requestStep != null)
        {
            url += $"&step={requestStep}";
        }

        if (requestTimeout != null)
        {
            url += $"&timeout={requestTimeout}";
        }

        if (requestLimit != null)
        {
            url += $"&limit={requestLimit}";
        }

        logger.LogInformation("Querying {HttpClientBaseAddress}{Url}", httpClient.BaseAddress, url);
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        logger.LogInformation("Response received from {HttpClientBaseAddress}{Url}", httpClient.BaseAddress, url);
        await using var resultStream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<PrometheusResultDto>(resultStream);
        return result;
    }

    private static string GetPrometheusConfigYaml(bool duplicateFirst = false)
    {
        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var yamlToEditFolder = Path.Combine(
            currentDirectory,
            "Containerization/Clients/Orchestration/FileTemplates");
        var yamlToEdit = DockerLocalStorageHelper.FileCopyLocation(
            Path.Combine(yamlToEditFolder, "prometheus_edited.yml"));
        if (!duplicateFirst)
        {
            return yamlToEdit;
        }

        if (File.Exists(yamlToEdit))
        {
            File.Delete(yamlToEdit);
        }

        var yamlToEditOriginal = Path.Combine(yamlToEditFolder, "prometheus.yml");
        if (!File.Exists(yamlToEditOriginal))
        {
            throw new FileNotFoundException("Prometheus config template not found.", yamlToEditOriginal);
        }

        Console.WriteLine($"Copying file {yamlToEditOriginal} to {yamlToEdit}");
        File.Copy(yamlToEditOriginal, yamlToEdit, true);
        return yamlToEdit;
    }

    private static string GetLokiConfigYaml()
    {
        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var yamlToEdit = Path.Combine(
            currentDirectory,
            "Containerization/Clients/Orchestration/FileTemplates/loki-config.yaml");
        return yamlToEdit;
    }

    private static async Task<string> CreateWebConfigForPasswordAndReturnFilename(string password)
    {
        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var yamlTemplate = Path.Combine(
            currentDirectory,
            "Containerization/Clients/Orchestration/FileTemplates/web_template.yml");
        var yamlDestination = DockerLocalStorageHelper.FileCopyLocation(Path.Combine(
            currentDirectory,
            "Containerization/Clients/Orchestration/FileTemplates/web.yml"));
        //first copy into web.yml then just string.format as there is {0} for bcrypt hashed password of password
        File.Copy(yamlTemplate, yamlDestination, true);
        var yaml = await File.ReadAllTextAsync(yamlDestination);
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        yaml = string.Format(yaml, hashedPassword);
        await File.WriteAllTextAsync(yamlDestination, yaml);
        return yamlDestination;
    }

    private async Task RestartOrCreateLoki()
    {
        var networks = await client.Networks.ListNetworksAsync();
        if (networks.All(n => n.Name != DockerNamingHelper.ObservabilityNetworkName))
        {
            logger.LogInformation("Observability network does not exist. Creating...");
            await client.Networks.CreateNetworkAsync(new NetworksCreateParameters
            {
                Name = DockerNamingHelper.ObservabilityNetworkName, Driver = "bridge"
            });
        }

        if (networks.All(n => n.Name != DockerNamingHelper.LokiNetworkName))
        {
            logger.LogInformation("Loki network does not exist. Creating...");
            await client.Networks.CreateNetworkAsync(new NetworksCreateParameters
            {
                Name = DockerNamingHelper.LokiNetworkName, Driver = "bridge"
            });
        }

        await client.Volumes
            .CreateAsync(new VolumesCreateParameters { Name = DockerNamingHelper.LokiVolumeName });

        await client.Images.CreateImageAsync(
            new ImagesCreateParameters { FromImage = LokiImageName, Tag = "latest" },
            null,
            new Progress<JSONMessage>());
        try
        {
            var container = await client.Containers.InspectContainerAsync(DockerNamingHelper.LokiContainerName);
            if (container.State.Running)
            {
                await client.Containers
                    .RestartContainerAsync(
                        DockerNamingHelper.LokiContainerName,
                        new ContainerRestartParameters());
                logger.LogInformation("Loki container is running, Restarting.");
                return;
            }

            await client.Containers
                .StartContainerAsync(DockerNamingHelper.LokiContainerName, new ContainerStartParameters());
            logger.LogInformation("Loki container is not running. Starting...");
            return;
        }
        catch (Exception ex)
        {
            logger.LogInformation(ex, "Loki container does not exist. Creating...");
        }

        var createContainerParameters = new CreateContainerParameters
        {
            Name = DockerNamingHelper.LokiContainerName,
            Image = LokiImageName,
            HostConfig =
                new HostConfig
                {
                    RestartPolicy =
                        new RestartPolicy { Name = RestartPolicyKind.Always, MaximumRetryCount = 0 },
                    Binds =
                        new List<string>
                        {
                            $"{DockerNamingHelper.LokiVolumeName}:/loki",
                            $"{DockerLocalStorageHelper.CopyAndResolvePersistedPath(GetLokiConfigYaml())}:/etc/loki/loki-config.yaml"
                        },
                    PortBindings = InDocker
                        ? new Dictionary<string, IList<PortBinding>>()
                        : new Dictionary<string, IList<PortBinding>>
                        {
                            { "3100/tcp", new List<PortBinding> { new PortBinding { HostPort = "3100" } } }
                        }
                },
            ExposedPorts = new Dictionary<string, EmptyStruct> { { "3100/tcp", default } },
            NetworkingConfig = new NetworkingConfig
            {
                EndpointsConfig = new Dictionary<string, EndpointSettings>
                {
                    { DockerNamingHelper.ObservabilityNetworkName, new EndpointSettings() },
                    { DockerNamingHelper.LokiNetworkName, new EndpointSettings() }
                }
            },
            Cmd = new List<string> { "--config.file=/etc/loki/loki-config.yaml" }
        };
        var containerCreated = await client.Containers.CreateContainerAsync(createContainerParameters);
        await client.Containers
            .StartContainerAsync(containerCreated.ID, new ContainerStartParameters());
        logger.LogInformation("Loki container created and started.");
    }

    private async Task RestartOrCreatePrometheus()
    {
        var networks = await client.Networks.ListNetworksAsync();
        if (networks.All(n => n.Name != DockerNamingHelper.ObservabilityNetworkName))
        {
            logger.LogInformation("Observability network does not exist. Creating...");
            await client.Networks.CreateNetworkAsync(new NetworksCreateParameters
            {
                Name = DockerNamingHelper.ObservabilityNetworkName, Driver = "bridge"
            });
        }

        if (networks.All(n => n.Name != DockerNamingHelper.PrometheusNetworkName))
        {
            logger.LogInformation("Prometheus network does not exist. Creating...");
            await client.Networks.CreateNetworkAsync(new NetworksCreateParameters
            {
                Name = DockerNamingHelper.PrometheusNetworkName, Driver = "bridge"
            });
        }

        await client.Volumes
            .CreateAsync(new VolumesCreateParameters { Name = DockerNamingHelper.PrometheusVolumeName });

        await client.Images.CreateImageAsync(
            new ImagesCreateParameters { FromImage = PrometheusImageName, Tag = "latest" },
            null,
            new Progress<JSONMessage>());
        try
        {
            var container = await client.Containers.InspectContainerAsync(DockerNamingHelper.PrometheusContainerName);
            if (container.State.Running)
            {
                await client.Containers
                    .RestartContainerAsync(
                        DockerNamingHelper.PrometheusContainerName,
                        new ContainerRestartParameters());
                logger.LogInformation("Prometheus container is running, Restarting.");
                return;
            }

            await client.Containers
                .StartContainerAsync(DockerNamingHelper.PrometheusContainerName, new ContainerStartParameters());
            logger.LogInformation("Prometheus container is not running. Starting...");
            return;
        }
        catch (Exception ex)
        {
            logger.LogInformation(ex, "Prometheus container does not exist. Creating...");
        }

        var createContainerParameters = new CreateContainerParameters
        {
            Name = DockerNamingHelper.PrometheusContainerName,
            Image = PrometheusImageName,
            HostConfig = new HostConfig
            {
                RestartPolicy = new RestartPolicy { Name = RestartPolicyKind.Always, MaximumRetryCount = 0 },
                Binds = new List<string>
                {
                    $"{DockerLocalStorageHelper.CopyAndResolvePersistedPath(GetPrometheusConfigYaml(true))}:/etc/prometheus/prometheus.yml",
                    $"{DockerNamingHelper.PrometheusVolumeName}:/prometheus",
                    $"{DockerLocalStorageHelper.CopyAndResolvePersistedPath(await CreateWebConfigForPasswordAndReturnFilename(_prometheusPassword))}:/etc/prometheus/web.yml",
                },
                PortBindings = InDocker
                    ? new Dictionary<string, IList<PortBinding>>()
                    : new Dictionary<string, IList<PortBinding>>
                    {
                        { "9090/tcp", new List<PortBinding> { new PortBinding { HostPort = "9090" } } }
                    }
            },
            ExposedPorts = new Dictionary<string, EmptyStruct> { { "9090/tcp", default } },
            NetworkingConfig = new NetworkingConfig
            {
                EndpointsConfig = new Dictionary<string, EndpointSettings>
                {
                    { DockerNamingHelper.ObservabilityNetworkName, new EndpointSettings() },
                    { DockerNamingHelper.PrometheusNetworkName, new EndpointSettings() }
                }
            },
            Cmd = new List<string>
            {
                "--web.config.file=/etc/prometheus/web.yml", "--config.file=/etc/prometheus/prometheus.yml"
            }
        };
        var containerCreated = await client.Containers.CreateContainerAsync(createContainerParameters);
        await client.Containers
            .StartContainerAsync(containerCreated.ID, new ContainerStartParameters());
        logger.LogInformation("Prometheus container created and started.");
    }

    public async Task RestartOrCreateCadvisor()
    {
        var networks = await client.Networks.ListNetworksAsync();
        if (networks.All(n => n.Name != DockerNamingHelper.ObservabilityNetworkName))
        {
            logger.LogInformation("Observability network does not exist. Creating...");
            await client.Networks.CreateNetworkAsync(new NetworksCreateParameters
            {
                Name = DockerNamingHelper.ObservabilityNetworkName, Driver = "bridge"
            });
        }

        await client.Images.CreateImageAsync(
            new ImagesCreateParameters { FromImage = CadvisorImageName, Tag = "latest" },
            null,
            new Progress<JSONMessage>());
        try
        {
            var container = await client.Containers.InspectContainerAsync(DockerNamingHelper.CadvisorContainerName);
            if (container.State.Running)
            {
                await client.Containers.RestartContainerAsync(DockerNamingHelper.CadvisorContainerName,
                    new ContainerRestartParameters());
                logger.LogInformation("Cadvisor container is running, Restarting.");
                return;
            }

            await client.Containers.StartContainerAsync(DockerNamingHelper.CadvisorContainerName,
                new ContainerStartParameters());
            logger.LogInformation("Cadvisor container is not running. Starting...");
            return;
        }
        catch (Exception ex)
        {
            logger.LogInformation(ex, "Cadvisor container does not exist. Creating...");
        }

        var createContainerParameters = new CreateContainerParameters
        {
            Name = DockerNamingHelper.CadvisorContainerName,
            Image = CadvisorImageName,
            HostConfig = new HostConfig
            {
                RestartPolicy = new RestartPolicy { Name = RestartPolicyKind.Always, MaximumRetryCount = 0 },
                Binds = new List<string>
                {
                    "/:/rootfs:ro", "/var/run:/var/run:rw", "/sys:/sys:ro",
                    // "/var/lib/docker/:/var/lib/docker:ro" maybe on linux
                },
                Privileged = true,
                Devices = new List<DeviceMapping>
                {
                    new() { PathOnHost = "/dev/kmsg", PathInContainer = "/dev/kmsg", CgroupPermissions = "rwm" }
                }
            },
            NetworkingConfig = new NetworkingConfig
            {
                EndpointsConfig = new Dictionary<string, EndpointSettings>
                {
                    { DockerNamingHelper.ObservabilityNetworkName, new EndpointSettings() }
                }
            },
        };
        var containerCreated = await client.Containers.CreateContainerAsync(createContainerParameters);
        await client.Containers.StartContainerAsync(containerCreated.ID, new ContainerStartParameters());
        logger.LogInformation("Cadvisor container created and started.");
    }
}
