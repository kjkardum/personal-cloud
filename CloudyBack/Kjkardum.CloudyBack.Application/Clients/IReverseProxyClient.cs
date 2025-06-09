namespace Kjkardum.CloudyBack.Application.Clients;

public interface IReverseProxyClient
{
    Task AddProxyConfiguration(Guid proxiedContainerId, int proxiedPort, string hostName, bool useHttps);

    Task AddProxyConfigurationByContainerName(
        string containerName,
        string networkName,
        int proxiedPort,
        string hostName,
        bool useHttps);
    Task RemoveProxyConfiguration(Guid proxiedContainerId, int proxiedPort, string hostName, bool useHttps);

    Task RemoveProxyConfigurationByContainerName(
        string containerName,
        int proxiedPort,
        string hostName,
        bool useHttps);
    Task EnsureCreated();
}
