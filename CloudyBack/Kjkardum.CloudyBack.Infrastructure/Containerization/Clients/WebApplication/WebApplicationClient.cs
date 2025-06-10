using ApiExceptions.Exceptions;
using Docker.DotNet;
using Docker.DotNet.Models;
using ICSharpCode.SharpZipLib.Tar;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Domain.Enums;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Helpers;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text;

namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.WebApplication;

public class WebApplicationClient(DockerClient client, ILogger<WebApplicationClient> logger) : IWebApplicationClient
{
    private const string OtelCollectorImageName = "otel/opentelemetry-collector-contrib";

    public async Task BuildAndRunWebApplicationUsingGitRepo(
        Guid id,
        string name, //TODO use this !!!!!!!!!
        string repoUrl,
        string buildCommand,
        string runCommand,
        WebApplicationRuntimeType runtimeType,
        int port,
        List<string> environmentVariables,
        List<Guid> virtualNetworks)
    {
        logger.LogInformation("Building and running web application from repository: {RepoUrl}", repoUrl);
        await CreateSidecarTelemetryCollector(id, name);
        await BuildWebApplicationUsingGitRepo(id, repoUrl, buildCommand, environmentVariables, virtualNetworks);
        await RunWebApplicationUsingGitRepo(id, runtimeType, runCommand, port, environmentVariables, virtualNetworks);
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

    private async Task CreateSidecarTelemetryCollector(Guid id, string appName)
    {
        try
        {
            var existingTelemetryContainer = await client.Containers.InspectContainerAsync(
                DockerNamingHelper.GetSidecarTelemetryName(id));
            if (!existingTelemetryContainer.State.Running)
            {
                logger.LogInformation("Telemetry container for ID: {Id} is not running. Starting it.", id);
                await client.Containers.StartContainerAsync(
                    DockerNamingHelper.GetSidecarTelemetryName(id), new ContainerStartParameters());
                logger.LogInformation("Telemetry container started for ID: {Id}.", id);
            }

            logger.LogInformation("Telemetry container for ID: {Id} is already running.", id);
            return;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating telemetry container");
            logger.LogInformation("No existing telemetry container found for ID: {Id}. Creating a new one.", id);
        }

        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                               throw new InvalidOperationException("Could not determine the current directory.");
        var collectorYamlTemplate = Path.Combine(
            currentDirectory,
            "Containerization/Clients/WebApplication/FileTemplates/collector.yml");
        await client.Images.CreateImageAsync(
            new ImagesCreateParameters { FromImage = OtelCollectorImageName, Tag = "latest" },
            null,
            new Progress<JSONMessage>());
        logger.LogInformation("Otel Collector Image pulled");
        await client.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Name = DockerNamingHelper.GetSidecarTelemetryName(id),
                Image = OtelCollectorImageName,
                NetworkingConfig =
                    new NetworkingConfig
                    {
                        EndpointsConfig = new Dictionary<string, EndpointSettings>()
                        {
                            { DockerNamingHelper.GetNetworkName(id), new EndpointSettings() },
                            { DockerNamingHelper.ObservabilityNetworkName, new EndpointSettings() }
                        }
                    },
                HostConfig =
                    new HostConfig
                    {
                        Binds = new List<string>
                        {
                            $"{DockerLocalStorageHelper.CopyAndResolvePersistedPath(collectorYamlTemplate)}:/conf/collector.yml",
                        },
                        RestartPolicy = new RestartPolicy { Name = RestartPolicyKind.Always, MaximumRetryCount = 0 }
                    },
                Env = new List<string>
                {
                    $"DATA_SOURCE_NAME={appName}",
                    $"OTLP_ENDPOINT=http://{DockerNamingHelper.LokiContainerName}:3100/otlp"
                },
                Cmd = new List<string> { "--config=/conf/collector.yml" }
            });
        logger.LogInformation("Otel Collector Container created");
        await client.Containers
            .StartContainerAsync(DockerNamingHelper.GetSidecarTelemetryName(id), new ContainerStartParameters());
        logger.LogInformation("Otel Collector Container started");
    }

    private async Task RunWebApplicationUsingGitRepo(
        Guid id,
        WebApplicationRuntimeType runtimeType,
        string runCommand,
        int port,
        List<string> environmentVariables,
        List<Guid> virtualNetworks)
    {
        var runnerImage = runtimeType switch
        {
            WebApplicationRuntimeType.DotNet => "mcr.microsoft.com/dotnet/aspnet:9.0-alpine",
            WebApplicationRuntimeType.NodeJs => "node:20",
            WebApplicationRuntimeType.Python => "python:bookworm",
            _ => throw new NotSupportedException($"Runtime type {runtimeType} is not supported.")
        };
        var volumeName = DockerNamingHelper.GetVolumeName(id);
        var appContainer = DockerNamingHelper.GetContainerName(id);
        logger.LogInformation("Running web application with ID: {Id} using command: {RunCommand}", id, runCommand);
        await RemoveExistingContainer(id);
        await client.Images.CreateImageAsync(
            new ImagesCreateParameters { FromImage = runnerImage, Tag = "latest" },
            null,
            new Progress<JSONMessage>());
        await CreateNetworkForApplicationIfNotExists(id);
        var networksDictionary = virtualNetworks.ToDictionary(
            DockerNamingHelper.GetVirtualNetworkResourceName,
            _ => new EndpointSettings());
        networksDictionary[DockerNamingHelper.GetNetworkName(id)] = new EndpointSettings();
        var containerParameters = new CreateContainerParameters
        {
            Image = runnerImage,
            Name = appContainer,
            HostConfig =
                new HostConfig
                {
                    LogConfig = DockerLoggingHelper.DefaultLogConfig(id),
                    Binds = [$"{volumeName}:/appsrc"],
                    RestartPolicy = new RestartPolicy { Name = RestartPolicyKind.Always, MaximumRetryCount = 0 }
                },
            NetworkingConfig = new NetworkingConfig { EndpointsConfig = networksDictionary },
            Env = [..environmentVariables, DockerLoggingHelper.LogEnvironmentVariable(id)],
            ExposedPorts = new Dictionary<string, EmptyStruct> { { $"{port}/tcp", default } },
            Cmd = new List<string> { "bash", "-c", $"cd /appsrc && {runCommand}" }
        };
        var containerReference = await client.Containers.CreateContainerAsync(containerParameters);
        await client.Containers.StartContainerAsync(containerReference.ID, new ContainerStartParameters());
    }

    private async Task CreateNetworkForApplicationIfNotExists(Guid id)
    {
        var networks = await client.Networks.ListNetworksAsync();
        if (networks.All(n => n.Name != DockerNamingHelper.GetNetworkName(id)))
        {
            logger.LogInformation($"Web application {id} network does not exist. Creating...");
            await client.Networks.CreateNetworkAsync(new NetworksCreateParameters
            {
                Name = DockerNamingHelper.GetNetworkName(id), Driver = "bridge"
            });
        }
    }

    private async Task RemoveExistingContainer(Guid id)
    {
        var appContainer = DockerNamingHelper.GetContainerName(id);
        try
        {
            var existingContainer = await client.Containers.InspectContainerAsync(appContainer);
            logger.LogInformation("Removing existing container: {ContainerName}", appContainer);
            await client.Containers.RemoveContainerAsync(
                appContainer,
                new ContainerRemoveParameters { Force = true });
        }
        catch (Exception ex)
        {
            logger.LogInformation("No existing container found with name: {ContainerName}", appContainer);
        }
    }

    public async Task<string> RecreateVolumeForWebApplication(Guid id)
    {
        var volumeName = DockerNamingHelper.GetVolumeName(id);
        logger.LogInformation("Recreating volumeName for web application: {VolumeName}", volumeName);
        try
        {
            await client.Volumes.RemoveAsync(volumeName, true);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to remove volume {VolumeName}. It may not exist.", volumeName);
        }

        await client.Volumes.CreateAsync(new VolumesCreateParameters { Name = volumeName });
        return volumeName;
    }

    public async Task BuildWebApplicationUsingGitRepo(
        Guid id,
        string repoUrl,
        string buildCommand,
        List<string> environmentVariables,
        List<Guid> virtualNetworks)
    {
        logger.LogInformation(
            "Building web application with ID: {Id} from repository: {RepoUrl} using command: {BuildCommand}", id,
            repoUrl, buildCommand);
        await RemoveExistingContainer(id);
        var volumeName = await RecreateVolumeForWebApplication(id);
        var builderImageName = await GetBuilderDockerImage();
        var container = new CreateContainerParameters
        {
            Image = builderImageName,
            Name = DockerNamingHelper.GetContainerName(id) + "builder",
            HostConfig =
                new HostConfig
                {
                    LogConfig = DockerLoggingHelper.DefaultLogConfig(id), Binds = [$"{volumeName}:/appsrc"]
                },
            NetworkingConfig = new NetworkingConfig
            {
                EndpointsConfig = virtualNetworks.ToDictionary(
                    DockerNamingHelper.GetVirtualNetworkResourceName,
                    _ => new EndpointSettings())
            },
            Env = [..environmentVariables, DockerLoggingHelper.LogEnvironmentVariable(id)],
            Cmd = new List<string>()
            {
                "bash",
                "-c",
                string.IsNullOrWhiteSpace(buildCommand)
                    ? $"git clone {repoUrl} ."
                    : $"git clone {repoUrl} . && {buildCommand}"
            }
        };
        var builderContainerReference = await client.Containers.CreateContainerAsync(container);
        try
        {
            logger.LogInformation("Created container for building web application: {ContainerId}",
                builderContainerReference.ID);
            await client.Containers.StartContainerAsync(builderContainerReference.ID, new ContainerStartParameters());
            logger.LogInformation("Started container for building web application: {ContainerId}",
                builderContainerReference.ID);
            var waitTask = client.Containers.WaitContainerAsync(builderContainerReference.ID);
            var timerTask = Task.Delay(TimeSpan.FromMinutes(5));
            var completedTask = await Task.WhenAny(waitTask, timerTask);
            if (completedTask == waitTask)
            {
                var waitResult = await waitTask;
                if (waitResult.StatusCode != 0)
                {
                    logger.LogError("Container for building web application failed with status code: {StatusCode}",
                        waitResult.StatusCode);
                    throw new BadRequestException(
                        $"Building web application failed with status code: {waitResult.StatusCode}");
                }

                logger.LogInformation("Container for building web application completed successfully.");
            }
            else
            {
                logger.LogError("Building web application timed out after 5 minutes.");
                throw new BadRequestException("Building web application timed out after 5 minutes.");
            }
        }
        finally
        {
            try
            {
                await client.Containers.RemoveContainerAsync(
                    builderContainerReference.ID,
                    new ContainerRemoveParameters { Force = true });
                logger.LogInformation("Removed container for building web application: {ContainerId}",
                    builderContainerReference.ID);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to remove container {ContainerId} after building web application.",
                    builderContainerReference.ID);
            }
        }
    }

    private async Task<string> GetBuilderDockerImage()
    {
        //previous this was not await using
        await using var builderDockerfileContext = GetWebApplicationBuilderDockerfileContext();
        await client.Images.BuildImageFromDockerfileAsync(
            new ImageBuildParameters
            {
                Tags = [DockerNamingHelper.WebApplicationBuilderImageName],
                Dockerfile = "WebApplicationBuilder.Dockerfile",
            },
            builderDockerfileContext,
            null,
            new Dictionary<string, string>(),
            new Progress<JSONMessage>());
        //previously here was manual close of stream
        return DockerNamingHelper.WebApplicationBuilderImageName;
    }

    private static Stream GetWebApplicationBuilderDockerfileContext()
        => DockerBuilderHelper.GetDockerfileContext("WebApplication/Dockerfiles/WebApplicationBuilder.Dockerfile");
}
