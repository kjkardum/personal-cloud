using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Clients;

public interface IGeneralContainerStatusClient
{
    Task<DockerContainer> GetContainerStateAsync(Guid id);
    Task DeleteContainerAsync(Guid requestId);
}
