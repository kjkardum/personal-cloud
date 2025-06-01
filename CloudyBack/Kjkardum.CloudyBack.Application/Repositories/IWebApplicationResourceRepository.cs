using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Repositories;

public interface IWebApplicationResourceRepository
{
    Task<WebApplicationResource> Create(WebApplicationResource kafkaClusterResource);
    Task<WebApplicationResource?> GetById(Guid id);
    Task<WebApplicationResource?> Update(WebApplicationResource resource);
    Task Delete(WebApplicationResource resource);
    Task UpsertConfigurationEntry(WebApplicationConfigurationEntry configurationEntry);
    Task DeleteConfigurationEntry(WebApplicationConfigurationEntry configurationEntry);
}
