namespace Kjkardum.CloudyBack.Application.Clients;

public interface IVirtualNetworkClient
{
    Task CreateVirtualNetworkAsync(Guid id);
    Task AttachToVirtualNetworkAsync(Guid virtualNetworkId, Guid resourceId);
}
