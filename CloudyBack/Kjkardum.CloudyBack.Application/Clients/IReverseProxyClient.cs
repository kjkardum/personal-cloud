namespace Kjkardum.CloudyBack.Application.Clients;

public interface IReverseProxyClient
{
    Task AddProxyConfiguration(Guid proxiedContainerId, int proxiedPort, string hostName, bool useHttps);
    Task RemoveProxyConfiguration(Guid proxiedContainerId, int proxiedPort, string hostName, bool useHttps);
    Task EnsureCreated();
}
