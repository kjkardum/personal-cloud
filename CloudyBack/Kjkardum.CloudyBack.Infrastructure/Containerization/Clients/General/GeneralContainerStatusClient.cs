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

    public Task DeleteContainerAsync(Guid requestId)
    {
        var container = DockerNamingHelper.GetContainerName(requestId);
        return client.Containers.RemoveContainerAsync(
            container,
            new ContainerRemoveParameters
            {
                Force = true,
                RemoveVolumes = true
            });
    }
}
