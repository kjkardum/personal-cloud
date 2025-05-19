using Docker.DotNet;
using Docker.DotNet.Models;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;
using YamlDotNet.Serialization;

namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.Orchestration;

public class PrometheusClient(
    DockerClient client,
    ILogger<PrometheusClient> logger,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration): IPrometheusClient
{
    private readonly string _prometheusPassword = configuration["Keys:Prometheus"] ??
        throw new ArgumentNullException("Prometheus password is not set in the configuration");
    private const string PrometheusImageName = "quay.io/prometheus/prometheus";
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
    public async Task AttachCollector(Guid id, string jobName)
    {
        //TODO: Dodati persistence volume
        var containerName = DockerNamingHelper.GetSidecarTelemetryName(id);
        var url = $"{containerName}:8889";
        var yamlToEdit = GetConfigYaml();
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
    }

    public async Task<PrometheusResultDto?> QueryRange(
        string query,
        DateTime requestStart,
        DateTime requestEnd,
        string? requestStep,
        string? requestTimeout,
        int? requestLimit)
    {
        var httpClient = httpClientFactory.CreateClient("Prometheus");
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
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        using var resultStream = await response.Content.ReadAsStreamAsync();
        var result = await System.Text.Json.JsonSerializer.DeserializeAsync<PrometheusResultDto>(resultStream);
        return result;
    }

    private static string GetConfigYaml()
    {
        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var yamlToEdit = Path.Combine(
            currentDirectory,
            "Containerization/Clients/Orchestration/FileTemplates/prometheus.yml");
        return yamlToEdit;
    }

    private static async Task<string> CreateWebConfigForPasswordAndReturnFilename(string password)
    {
        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var yamlTemplate = Path.Combine(
            currentDirectory,
            "Containerization/Clients/Orchestration/FileTemplates/web_template.yml");
        var yamlDestination = Path.Combine(
            currentDirectory,
            "Containerization/Clients/Orchestration/FileTemplates/web.yml");
        //first copy into web.yml then just string.format as there is {0} for bcrypt hashed password of password
        File.Copy(yamlTemplate, yamlDestination, true);
        var yaml = await File.ReadAllTextAsync(yamlDestination);
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        yaml = string.Format(yaml, hashedPassword);
        await File.WriteAllTextAsync(yamlDestination, yaml);
        return yamlDestination;
    }

    private async Task RestartOrCreatePrometheus()
    {
        var networks = await client.Networks.ListNetworksAsync();
        if (networks.All(n => n.Name != DockerNamingHelper.ObservabilityNetworkName))
        {
            logger.LogInformation("Observability network does not exist. Creating...");
            await client.Networks.CreateNetworkAsync(new NetworksCreateParameters
                {
                    Name = DockerNamingHelper.ObservabilityNetworkName,
                    Driver = "bridge"
                });
        }

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
                AutoRemove = true,
                Binds = new List<string>
                {
                    $"{GetConfigYaml()}:/etc/prometheus/prometheus.yml",
                    $"{await CreateWebConfigForPasswordAndReturnFilename(_prometheusPassword)}:/etc/prometheus/web.yml",
                },
                /*REMOVE PORT BINDING TODO*/
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { "9090/tcp", new List<PortBinding> { new PortBinding { HostPort = "9090" } } }
                }
            },
            ExposedPorts = new Dictionary<string, EmptyStruct>
            {
                { "9090/tcp", default }
            },
            /*END REMOVE PORT BINDING TODO*/
            NetworkingConfig = new NetworkingConfig
            {
                EndpointsConfig = new Dictionary<string, EndpointSettings>
                {
                    { DockerNamingHelper.ObservabilityNetworkName, new EndpointSettings() }
                }
            },
            Cmd = new List<string>
            {
                "--web.config.file=/etc/prometheus/web.yml",
                "--config.file=/etc/prometheus/prometheus.yml"
            }
        };
        var containerCreated = await client.Containers.CreateContainerAsync(createContainerParameters);
        await client.Containers
            .StartContainerAsync(containerCreated.ID, new ContainerStartParameters());
        logger.LogInformation("Prometheus container created and started.");
    }
}
