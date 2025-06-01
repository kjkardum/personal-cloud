using Docker.DotNet;
using Docker.DotNet.Models;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Helpers;
using Microsoft.Extensions.Logging;

namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.VirtualNetwork;

public class VirtualNetworkClient(DockerClient client, ILogger<VirtualNetworkClient> logger): IVirtualNetworkClient
{
    public async Task CreateVirtualNetworkAsync(Guid id)
    {
        logger.LogInformation($"Creating virtual network {id}");
        await client.Networks.CreateNetworkAsync(new NetworksCreateParameters
            {
                Name = DockerNamingHelper.GetVirtualNetworkResourceName(id),
                Driver = "bridge"
            });
        logger.LogInformation("Virtual network created with ID: {Id}", id);
    }

    public async Task AttachToVirtualNetworkAsync(Guid virtualNetworkId, Guid resourceId)
    {
        logger.LogInformation(
            "Attaching resource {ResourceId} to virtual network {VirtualNetworkId}",
            resourceId,
            virtualNetworkId);
        var networkName = DockerNamingHelper.GetVirtualNetworkResourceName(virtualNetworkId);
        var containerName = DockerNamingHelper.GetContainerName(resourceId);

        var network = await client.Networks.InspectNetworkAsync(networkName);

        // Connect the container to the network
        await client.Networks.ConnectNetworkAsync(
            network.ID,
            new NetworkConnectParameters
                {
                    Container = containerName,
                    EndpointConfig = new EndpointSettings()
                });

        logger.LogInformation(
            "Resource {ResourceId} attached to virtual network {VirtualNetworkId}",
            resourceId,
            virtualNetworkId);
    }
}
