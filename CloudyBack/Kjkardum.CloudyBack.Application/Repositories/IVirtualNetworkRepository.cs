using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Repositories;

public interface IVirtualNetworkRepository
{
    Task<VirtualNetworkResource?> GetById(Guid id);
    Task AttachVirtualNetwork(Guid virtualNetworkId, Guid resourceId);
    Task<(IEnumerable<VirtualNetworkResource>, int)> GetPaginated(PaginatedRequest pagination);
    Task<VirtualNetworkResource> Create(VirtualNetworkResource virtualNetworkResource);
}
