using Docker.DotNet;
using Docker.DotNet.Models;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Domain.Entities;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Helpers;
using Microsoft.Extensions.Logging;

namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.General;

public class GeneralContainerStatusClient(DockerClient client, ILogger<GeneralContainerStatusClient> logger)
    : IGeneralContainerStatusClient
{
    public async Task<DockerContainer> GetContainerStateAsync(Guid id)
    {
        var container = await client.Containers.InspectContainerAsync(DockerNamingHelper.GetContainerName(id));
        return new DockerContainer
        {
            ContainerId = container.ID,
            StateRunning = container.State.Running,
            StatePaused = container.State.Paused,
            StateRestarting = container.State.Restarting,
            StateError = container.State.Error,
            StateStartedAt = string.IsNullOrEmpty(container.State.StartedAt)
                ? null
                : DateTime.Parse(container.State.StartedAt),
            StateFinishedAt = string.IsNullOrEmpty(container.State.FinishedAt)
                ? null
                : DateTime.Parse(container.State.FinishedAt)
        };
    }

    public async Task DeleteContainerAsync(Guid requestId)
    {
        var container = DockerNamingHelper.GetContainerName(requestId);
        try
        {
            await client.Containers.RemoveContainerAsync(
                container,
                new ContainerRemoveParameters
                {
                    Force = true,
                    RemoveVolumes = true
                });
            logger.LogInformation("Successfully removed container {container}.", container);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to remove container {container}, it may not exist.", container);
        }
        try
        {
            var telemetryContainer = DockerNamingHelper.GetSidecarTelemetryName(requestId);
            await client.Containers.RemoveContainerAsync(
                telemetryContainer,
                new ContainerRemoveParameters
                {
                    Force = true,
                    RemoveVolumes = true
                });
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to remove telemetry container for {id}, it may not exist.", requestId);
        }
        try
        {
            var volumeName = DockerNamingHelper.GetVolumeName(requestId);
            await client.Volumes.RemoveAsync(volumeName, true);
            logger.LogInformation("Successfully removed telemetry container for {id}.", requestId);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to remove volume for {id}, it may not exist.", requestId);
        }
        try
        {
            var networkName = DockerNamingHelper.GetNetworkName(requestId);
            await client.Networks.DeleteNetworkAsync(networkName);
            logger.LogInformation("Network {networkName} removed successfully.", networkName);
            var networkPruneResponse = await client.Networks.PruneNetworksAsync();
            logger.LogInformation("Network prune response: {response}", networkPruneResponse);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to remove network for {id}, it may not exist.", requestId);
        }
    }
}
