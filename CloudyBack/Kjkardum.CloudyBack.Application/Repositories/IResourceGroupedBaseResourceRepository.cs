using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Repositories;


public interface IResourceGroupedBaseResourceRepository
{
    Task<ResourceGroupedBaseResource?> GetByIdAsync(Guid id);
}
