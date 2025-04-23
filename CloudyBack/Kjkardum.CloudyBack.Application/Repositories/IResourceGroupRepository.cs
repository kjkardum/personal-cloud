using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Repositories;

public interface IResourceGroupRepository
{
    Task<ResourceGroup> Create(ResourceGroup resourceGroup);
    Task<ResourceGroup?> GetById(Guid id);
    Task<(IEnumerable<ResourceGroup>, int)> GetPaginated(PaginatedRequest pagination);
}
