using Docker.DotNet;
using Docker.DotNet.Models;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.ReverseProxy.Dtos;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Helpers;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;

namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.ReverseProxy;

public class CaddyReverseProxyClient(DockerClient client, ILogger<CaddyReverseProxyClient> logger) : IReverseProxyClient
{
    public async Task AddProxyConfiguration(Guid proxiedContainerId, int proxiedPort, string hostName, bool useHttps)
    {
        //has layer4 plugin added, prepared in CurrentDirectory + Containerization/Clients/ReverseProxy/Dockerfiles/Caddy.Dockerfile
        //todo proxy database and other tcp connections using layer4 plugin
        await CreateCaddyContainerIfNotExistsAsync();
        await AttachCaddyToContainerNetwork(proxiedContainerId);
        await ConfigureCaddyProxyAsync(proxiedContainerId, proxiedPort, hostName, useHttps);
        await RestartCaddyContainerAsync();
    }

    private async Task RestartCaddyContainerAsync()
    {
        try
        {
            var caddyContainer = await client.Containers.InspectContainerAsync(DockerNamingHelper.CaddyContainerName);
            if (caddyContainer.State.Running)
            {
                await client.Containers.StopContainerAsync(
                    DockerNamingHelper.CaddyContainerName, new ContainerStopParameters());
            }

            await client.Containers.StartContainerAsync(DockerNamingHelper.CaddyContainerName,
                new ContainerStartParameters());
            logger.LogInformation("Caddy Container restarted successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to restart Caddy Container. Error: {Message}", ex.Message);
        }
    }

    private async Task ConfigureCaddyProxyAsync(
        Guid proxiedContainerId,
        int proxiedPort,
        string hostName,
        bool useHttps)
    {
        var fileToEdit = GetCaddyConfigFile(justReturn: true);
        var content = string.Empty;
        await using (var fileStream = new FileStream(fileToEdit, FileMode.Open, FileAccess.ReadWrite))
        using (var reader = new StreamReader(fileStream))
        {
            content = await reader.ReadToEndAsync();
        }

        var caddyConfigDto = JsonSerializer.Deserialize<CaddyConfigDto>(content);
        if (caddyConfigDto == null)
        {
            throw new InvalidOperationException("Failed to deserialize Caddy configuration.");
        }

        var serverName = $"{DockerNamingHelper.GetContainerName(proxiedContainerId)}:{proxiedPort}";
        var hostPort = useHttps ? ":443" : ":80";
        var serverIndex = caddyConfigDto.apps.http.servers.Count;
        caddyConfigDto.apps.http.servers.Add(
            $"srv{serverIndex}",
            new Srv
            {
                listen = new List<string> { hostPort },
                routes =
                    new List<RouteMatch>()
                    {
                        new()
                        {
                            match = [new Match { host = [hostName] }],
                            handle =
                                new List<HandleRoutes>
                                {
                                    new()
                                    {
                                        handler = "subroute",
                                        routes =
                                            new List<RouteInHandlerUpstream>
                                            {
                                                new()
                                                {
                                                    handle =
                                                        new List<HandleUpstreams>
                                                        {
                                                            new HandleUpstreams
                                                            {
                                                                handler = "reverse_proxy",
                                                                upstreams =
                                                                    new List<Upstream> { new() { dial = serverName } }
                                                            }
                                                        }
                                                }
                                            }
                                    }
                                },
                            terminal = true
                        }
                    },
                automatic_https = useHttps ? null : new AutomaticHttps { skip = new List<string> { hostName } }
            });
        //write back to file
        var contentNew = JsonSerializer.Serialize(caddyConfigDto);
        await using var fileStreamNew = new FileStream(fileToEdit, FileMode.Open, FileAccess.ReadWrite);
        await using var writer = new StreamWriter(fileStreamNew);
        await writer.WriteAsync(contentNew);
        logger.LogDebug("New configuration:\n{NewConfiguration}", contentNew);
    }

    private async Task AttachCaddyToContainerNetwork(Guid proxiedContainerId)
    {
        var networkName = DockerNamingHelper.GetNetworkName(proxiedContainerId);
        try
        {
            var network = await client.Networks.InspectNetworkAsync(networkName);
            await client.Networks.ConnectNetworkAsync(
                network.ID,
                new NetworkConnectParameters
                {
                    Container = DockerNamingHelper.CaddyContainerName, EndpointConfig = new EndpointSettings()
                });
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                "Network {NetworkName} does not exist or other failure. Error: {Message}",
                networkName,
                ex.Message);
        }
    }

    public async Task RemoveProxyConfiguration(
        Guid proxiedContainerId,
        int proxiedPort,
        string hostName,
        bool useHttps)
    {
        var fileToEdit = GetCaddyConfigFile(justReturn: true);
        var content = string.Empty;
        await using (var fileStream = new FileStream(fileToEdit, FileMode.Open, FileAccess.ReadWrite))
        using (var reader = new StreamReader(fileStream))
        {
            content = await reader.ReadToEndAsync();
        }

        var caddyConfigDto = JsonSerializer.Deserialize<CaddyConfigDto>(content);
        if (caddyConfigDto == null)
        {
            throw new InvalidOperationException("Failed to deserialize Caddy configuration.");
        }

        var serverName = $"{DockerNamingHelper.GetContainerName(proxiedContainerId)}:{proxiedPort}";
        var hostPort = useHttps ? ":443" : ":80";
        var server = caddyConfigDto.apps.http.servers
            .FirstOrDefault(srv => srv.Value.listen.Contains(hostPort)
                                   && srv.Value.routes.Any(r =>
                                       r.match.Any(m => m.host.Contains(hostName)) &&
                                       r.handle.Any(h => h.routes.Any(u =>
                                           u.handle.Any(up =>
                                               up.upstreams.Any(upstream => upstream.dial == serverName))))));
        if (server.Value == null)
        {
            logger.LogWarning("No matching server found for host {HostName} and port {Port}.", hostName, proxiedPort);
            return;
        }

        caddyConfigDto.apps.http.servers.Remove(server.Key);
        //write back to file
        var contentNew = JsonSerializer.Serialize(caddyConfigDto);
        await using var writer = new StreamWriter(fileToEdit, false);
        await writer.WriteAsync(contentNew);
        logger.LogInformation(
            "Removed proxy configuration for host {HostName} and port {Port} from Caddy configuration.",
            hostName, proxiedPort);
        logger.LogDebug("New configuration:\n{NewConfiguration}", contentNew);
    }

    private async Task CreateCaddyContainerIfNotExistsAsync()
    {
        try
        {
            var existingContainer = await client.Containers
                .InspectContainerAsync(DockerNamingHelper.CaddyContainerName);
            if (existingContainer.State.Running)
            {
                return;
            }

            await client.Containers
                .StartContainerAsync(DockerNamingHelper.CaddyContainerName, new ContainerStartParameters());
            logger.LogInformation("Caddy Container started");
        }
        catch (Exception ex)
        {
            logger.LogInformation("Caddy Container does not exist, creating a new one. Error:\n{Message}",
                ex.Message);
        }

        await RecreateCaddyVolume("data");
        await RecreateCaddyVolume("config");
        await CreateCaddyContainer();
    }

    private async Task CreateCaddyContainer()
    {
        var caddyImage = await GetBuilderDockerImage();
        var containerParameters = new CreateContainerParameters
        {
            Image = caddyImage,
            Name = DockerNamingHelper.CaddyContainerName,
            HostConfig = new HostConfig
            {
                Binds = new List<string>
                {
                    $"{DockerNamingHelper.CaddyVolumeName}data:/data",
                    $"{DockerNamingHelper.CaddyVolumeName}config:/config",
                    $"{DockerLocalStorageHelper.CopyAndResolvePersistedPath(GetCaddyConfigFile())}:/etc/caddy/caddy.json"
                },
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { "80/tcp", [new PortBinding { HostPort = "80" }] },
                    { "443/tcp", [new PortBinding { HostPort = "443" }] }
                }
            },
            ExposedPorts = new Dictionary<string, EmptyStruct> { { "80/tcp", default }, { "443/tcp", default }, },
            Cmd =
            [
                "caddy", "run", "--config", "/etc/caddy/caddy.json"
            ]
        };
        var containerReference = await client.Containers.CreateContainerAsync(containerParameters);
        await client.Containers.StartContainerAsync(containerReference.ID, new ContainerStartParameters());
        logger.LogInformation("Caddy Container created and started successfully.");
    }

    private static string GetCaddyConfigFile(bool justReturn = false)
    {
        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                               throw new InvalidOperationException("Could not determine the current directory.");
        var collectorCopiableTemplateFolder = Path.Combine(
            currentDirectory,
            "Containerization/Clients/ReverseProxy/FileTemplates");
        var fileToEdit = DockerLocalStorageHelper.FileCopyLocation(
            Path.Combine(collectorCopiableTemplateFolder, "caddy_edit.json"));
        var fileToEditOriginal = Path.Combine(collectorCopiableTemplateFolder, "caddy.json");
        if (justReturn)
        {
            return fileToEdit;
        }

        if (!File.Exists(fileToEditOriginal))
        {
            throw new FileNotFoundException("Caddy configuration file not found.", fileToEditOriginal);
        }

        if (File.Exists(fileToEdit))
        {
            File.Delete(fileToEdit);
        }

        File.Copy(fileToEditOriginal, fileToEdit, true);
        return fileToEdit;
    }

    private async Task RecreateCaddyVolume(string suffix)
    {
        try
        {
            await client.Volumes.RemoveAsync(DockerNamingHelper.CaddyVolumeName + suffix);
            logger.LogInformation("Caddy old Volume removed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogInformation("Caddy Volume does not exist, creating a new one. Error:\n{Message}", ex.Message);
        }

        await client.Volumes
            .CreateAsync(new VolumesCreateParameters { Name = DockerNamingHelper.CaddyVolumeName + suffix });
    }

    private async Task<string> GetBuilderDockerImage()
    {
        //previous this was not await using
        await using var builderDockerfileContext = GetCaddyBuilderDockerfileContext();
        await client.Images.BuildImageFromDockerfileAsync(
            new ImageBuildParameters
            {
                Tags = [DockerNamingHelper.CaddyBuilderImageName], Dockerfile = "Caddy.Dockerfile",
            },
            builderDockerfileContext,
            null,
            new Dictionary<string, string>(),
            new Progress<JSONMessage>());
        //previously here was manual close of stream
        return DockerNamingHelper.CaddyBuilderImageName;
    }

    private static Stream GetCaddyBuilderDockerfileContext()
        => DockerBuilderHelper.GetDockerfileContext("ReverseProxy/Dockerfiles/Caddy.Dockerfile");
}
