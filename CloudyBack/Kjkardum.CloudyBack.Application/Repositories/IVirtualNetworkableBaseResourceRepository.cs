using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Repositories;

public interface IVirtualNetworkableBaseResourceRepository
{
    Task<VirtualNetworkableBaseResource?> GetById(Guid id);
    Task AddProxyConfiguration(PublicProxyConfiguration publicProxyConfiguration);
    Task RemoveProxyConfiguration(Guid id);

}
