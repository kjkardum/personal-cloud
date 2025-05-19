using Docker.DotNet;
using Docker.DotNet.Models;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Helpers;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.Postgres;

public class PostgresServerClient(DockerClient client, ILogger<PostgresServerClient> logger) : IPostgresServerClient
{
    private const string ImageName = "postgres";
    private const string PostgresExporterImageName = "otel/opentelemetry-collector-contrib";

    public async Task CreateServerAsync(Guid id, int serverPort, string saUsername, string saPassword, string serverName)
    {
        await CreateCommonVolume(id);
        await CreateCommonNetwork(id);
        await CreateServerContainer(id, serverPort, saUsername, saPassword);
        await CreateSidecarTelemetryCollector(id, saUsername, saPassword, serverName);
    }

    private async Task CreateCommonVolume(Guid id)
    {
        await client.Volumes.CreateAsync(new VolumesCreateParameters { Name = DockerNamingHelper.GetVolumeName(id) });
        logger.LogInformation("Volume created");
    }

    private async Task CreateCommonNetwork(Guid id)
    {
        var networkName = DockerNamingHelper.GetNetworkName(id);
        var networks = await client.Networks.ListNetworksAsync();
        if (networks.Any(n => n.Name == networkName))
        {
            logger.LogInformation("Network already exists");
            return;
        }
        await client.Networks.CreateNetworkAsync(new NetworksCreateParameters
            {
                Name = networkName,
                Driver = "bridge"
            });
        logger.LogInformation("Network created");
    }

    private async Task CreateSidecarTelemetryCollector(Guid id, string saUsername, string saPassword, string serverName)
    {
        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var collectorYamlTemplate = Path.Combine(currentDirectory, "Containerization/Clients/Postgres/FileTemplates/collector.yml");
        await client.Images.CreateImageAsync(
            new ImagesCreateParameters { FromImage = PostgresExporterImageName, Tag = "latest" },
            null,
            new Progress<JSONMessage>());
        logger.LogInformation("Otel Postgres Collector Image pulled");
        await client.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Name = DockerNamingHelper.GetSidecarTelemetryName(id),
                Image = PostgresExporterImageName,
                NetworkingConfig = new NetworkingConfig()
                {
                    EndpointsConfig = new Dictionary<string, EndpointSettings>()
                    {
                        {DockerNamingHelper.GetNetworkName(id), new EndpointSettings()},
                        {DockerNamingHelper.ObservabilityNetworkName, new EndpointSettings()}
                    }
                },
                HostConfig = new HostConfig
                {
                    Binds = new List<string> { $"{collectorYamlTemplate}:/conf/collector.yml", },
                },
                Env = new List<string>
                {
                    $"DATA_SOURCE_URI={DockerNamingHelper.GetContainerName(id)}:5432",
                    $"DATA_SOURCE_USER={saUsername}",
                    $"DATA_SOURCE_PASS={saPassword}",
                    $"DATA_SOURCE_NAME={serverName}"
                },
                Cmd = new List<string> { "--config=/conf/collector.yml" }
            });
        logger.LogInformation("Otel Postgres Collector Container created");
        await client.Containers
            .StartContainerAsync(DockerNamingHelper.GetSidecarTelemetryName(id), new ContainerStartParameters());
        logger.LogInformation("Otel Postgres Collector Container started");
    }

    private async Task CreateServerContainer(Guid id, int serverPort, string saUsername, string saPassword)
    {
        await client.Images.CreateImageAsync(
            new ImagesCreateParameters { FromImage = ImageName, Tag = "latest" },
            null,
            new Progress<JSONMessage>());
        logger.LogInformation("Image pulled");
        await client.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Name = DockerNamingHelper.GetContainerName(id),
                Image = ImageName,
                NetworkingConfig = new NetworkingConfig()
                {
                    EndpointsConfig = new Dictionary<string, EndpointSettings>()
                    {
                        {DockerNamingHelper.GetNetworkName(id), new EndpointSettings()}
                    }
                },
                HostConfig = new HostConfig
                {
                    Binds = new List<string> { $"{DockerNamingHelper.GetVolumeName(id)}:/var/lib/postgresql/data", },
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        { "5432/tcp", new List<PortBinding> { new() { HostPort = $"{serverPort}" } } }
                    },
                },
                ExposedPorts = new Dictionary<string, EmptyStruct> { { "5432/tcp", default } },
                Env = new List<string> { $"POSTGRES_PASSWORD={saPassword}", $"POSTGRES_USER={saUsername}" },
            });
        logger.LogInformation("Container created");
        await client.Containers
            .StartContainerAsync(DockerNamingHelper.GetContainerName(id), new ContainerStartParameters());
        logger.LogInformation("Container started");
    }

    public async Task CreateDatabaseAsync(
        Guid id,
        string saUsername,
        string saPassword,
        string databaseName,
        string dbUsername,
        string dbPassword)
    {
        var container = await client.Containers.InspectContainerAsync(DockerNamingHelper.GetContainerName(id));
        logger.LogInformation("Container found");
        var DatabaseCreate = $"CREATE DATABASE {databaseName};";
        var UserCreateAndGrant = $"CREATE USER {dbUsername} WITH ENCRYPTED PASSWORD '{dbPassword}';" +
            $"GRANT ALL PRIVILEGES ON DATABASE {databaseName} TO {dbUsername};" +
            $"ALTER USER {dbUsername} SET search_path TO {databaseName};" +
            $"REVOKE ALL ON DATABASE postgres FROM PUBLIC;" +
            $"CREATE EXTENSION IF NOT EXISTS pg_stat_statements";
        var ConnectToNewDatabase = $"\\connect {databaseName};";
        var CreateSchema = $"CREATE SCHEMA IF NOT EXISTS {databaseName};" +
            $"REVOKE ALL ON DATABASE {databaseName} FROM PUBLIC;" +
            $"GRANT CONNECT ON DATABASE {databaseName} TO {dbUsername};" +
            $"GRANT ALL PRIVILEGES ON SCHEMA {databaseName} TO {dbUsername};" +
            $"ALTER DATABASE {databaseName} SET search_path TO {databaseName};" +
            $"CREATE EXTENSION IF NOT EXISTS pg_stat_statements;";
        var exec = await client.Exec.ExecCreateContainerAsync(
            container.ID,
            new ContainerExecCreateParameters
                {
                    AttachStdin = true,
                    AttachStdout = true,
                    AttachStderr = true,
                    Env = new List<string>
                    {
                        $"PGPASSWORD={saPassword}"
                    },
                    Cmd = new List<string>
                    {
                        "psql", "-U", saUsername,
                        "-P", "pager=off", "-P", "format=csv",
                        "-c", DatabaseCreate,
                        "-c", UserCreateAndGrant,
                        "-c", ConnectToNewDatabase,
                        "-c", CreateSchema
                    },
                    Tty = true,
                    Detach = false
                });
        logger.LogInformation("Exec created");
        var execStart = await client.Exec.StartAndAttachContainerExecAsync(exec.ID, true);
        var (stdOut, stdErr) = await execStart.ReadOutputToEndAsync(default);
        logger.LogInformation("Response: {StdOut}", stdOut);
    }

    public async Task StartServerAsync(Guid requestId)
    {
        logger.LogInformation($"Starting server {requestId}");
        var container = await client.Containers.InspectContainerAsync(DockerNamingHelper.GetContainerName(requestId));
        if (container.State.Running)
        {
            logger.LogInformation("Container is already running");
            return;
        }
        await client.Containers
            .StartContainerAsync(DockerNamingHelper.GetContainerName(requestId), new ContainerStartParameters());
        logger.LogInformation("Container started");
    }

    public async Task StopServerAsync(Guid requestId)
    {
        logger.LogInformation($"Stopping server {requestId}");
        var container = await client.Containers.InspectContainerAsync(DockerNamingHelper.GetContainerName(requestId));
        if (!container.State.Running)
        {
            logger.LogInformation("Container is already stopped");
            return;
        }
        await client.Containers
            .StopContainerAsync(DockerNamingHelper.GetContainerName(requestId), new ContainerStopParameters());
        logger.LogInformation("Container stopped");
    }

    public async Task RestartServerAsync(Guid requestId)
    {
        logger.LogInformation($"Restarting server {requestId}");
        var container = await client.Containers.InspectContainerAsync(DockerNamingHelper.GetContainerName(requestId));
        if (!container.State.Running)
        {
            logger.LogInformation("Container is already stopped");
            return;
        }
        await client.Containers
            .RestartContainerAsync(DockerNamingHelper.GetContainerName(requestId), new ContainerRestartParameters());
        logger.LogInformation("Container restarted");
    }
}
