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
    public async Task<DockerContainer?> GetContainerStateAsync(Guid id)
    {
        try
        {
            var container = await client.Containers.InspectContainerAsync(DockerNamingHelper.GetContainerName(id));
            return new DockerContainer
            {
                ContainerId = container.ID,
                ContainerName = container.Name,
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
        } catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to get container state for {id}.", id);
            return null;
        }
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

        for (var i = 0;; i++)
        {
            var scalableContainerName = DockerNamingHelper.GetScalableContainerName(requestId, i);
            try
            {
                await client.Containers.RemoveContainerAsync(
                    scalableContainerName,
                    new ContainerRemoveParameters
                    {
                        Force = true,
                        RemoveVolumes = true
                    });
                logger.LogInformation("Successfully removed scalable container {scalableContainerName}.", scalableContainerName);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to remove scalable container {scalableContainerName}, it may not exist.", scalableContainerName);
                logger.LogInformation("No more scalable containers found to remove for {id}.", requestId);
                break;
            }
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

        for (var i = 0;; i++)
        {
            var scalableVolumeName = DockerNamingHelper.GetScalableVolumeName(requestId, i);
            try
            {
                await client.Volumes.RemoveAsync(scalableVolumeName, true);
                logger.LogInformation("Successfully removed scalable volume {scalableVolumeName}.", scalableVolumeName);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to remove scalable volume {scalableVolumeName}, it may not exist.", scalableVolumeName);
                logger.LogInformation("No more scalable volumes found to remove for {id}.", requestId);
                break;
            }
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

    public async Task<DockerEnvironment> GetDockerEnvironmentAsync()
    {
        var containers = await client.Containers.ListContainersAsync(
            new ContainersListParameters { All = true });
        var images = await client.Images.ListImagesAsync(
            new ImagesListParameters { All = true });
        var networks = await client.Networks.ListNetworksAsync();
        var volumes = await client.Volumes.ListAsync();

        var environment = new DockerEnvironment
        {
            Containers = new List<DockerContainer>(),
            Images = new List<DockerImage>(),
            Networks = new List<DockerNetwork>(),
            Volumes = new List<DockerVolume>()
        };
        foreach (var container in containers)
        {
            var detailedContainer = await client.Containers.InspectContainerAsync(container.ID);
            environment.Containers.Add(new DockerContainer
                {
                    ContainerId = detailedContainer.ID,
                    ContainerName = detailedContainer.Name,
                    StateRunning = detailedContainer.State.Running,
                    StatePaused = detailedContainer.State.Paused,
                    StateRestarting = detailedContainer.State.Restarting,
                    StateError = detailedContainer.State.Error,
                    StateStartedAt = string.IsNullOrEmpty(detailedContainer.State.StartedAt)
                        ? null
                        : DateTime.Parse(detailedContainer.State.StartedAt),
                    StateFinishedAt = string.IsNullOrEmpty(detailedContainer.State.FinishedAt)
                        ? null
                        : DateTime.Parse(detailedContainer.State.FinishedAt),
                    NetworkIds = detailedContainer.NetworkSettings.Networks.Keys.ToList(),
                    VolumeIds = detailedContainer.Mounts.Select(m => m.Name).ToList()
                });
        }
        foreach (var image in images)
        {
            var imageDetails = await client.Images.InspectImageAsync(image.ID);
            environment.Images.Add(new DockerImage
                {
                    ImageId = imageDetails.ID,
                    Tag = imageDetails.RepoTags.FirstOrDefault() ?? "<none>",
                    Size = imageDetails.Size,
                    CreatedAt = imageDetails.Created
                });
        }
        foreach (var network in networks)
        {
            var detailedNetwork = await client.Networks.InspectNetworkAsync(network.ID);
            environment.Networks.Add(new DockerNetwork
                {
                    NetworkId = detailedNetwork.ID,
                    Name = detailedNetwork.Name,
                    ContainerIds = detailedNetwork.Containers?.Keys.ToList() ?? new List<string>()
                });
        }
        foreach (var volume in volumes.Volumes)
        {
            var detailedVolume = await client.Volumes.InspectAsync(volume.Name);
            environment.Volumes.Add(new DockerVolume
                {
                    Name = detailedVolume.Name,
                    CreatedAt = detailedVolume.CreatedAt
                });
        }
        return environment;
    }
}
