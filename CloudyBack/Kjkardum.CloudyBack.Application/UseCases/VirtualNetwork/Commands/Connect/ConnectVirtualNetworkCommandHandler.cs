using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kjkardum.CloudyBack.Application.UseCases.VirtualNetwork.Commands.Connect;

public class ConnectVirtualNetworkCommandHandler(
    IVirtualNetworkRepository virtualNetworkRepository,
    IBaseResourceRepository baseResourceRepository,
    IVirtualNetworkClient virtualNetworkClient,
    ILogger<ConnectVirtualNetworkCommandHandler> logger) : IRequestHandler<ConnectVirtualNetworkCommand>
{
    public async Task Handle(ConnectVirtualNetworkCommand request, CancellationToken cancellationToken)
    {
        var virtualNetwork = await virtualNetworkRepository.GetById(request.NetworkId);
        if (virtualNetwork == null)
        {
            throw new EntityNotFoundException($"Virtual network with ID {request.NetworkId} not found.");
        }

        var resource = await baseResourceRepository.GetById(request.ResourceId);
        if (resource == null)
        {
            throw new EntityNotFoundException($"Resource with ID {request.ResourceId} not found.");
        }

        await virtualNetworkRepository.AttachVirtualNetwork(virtualNetwork.Id, resource.Id);

        try
        {
            await virtualNetworkClient.AttachToVirtualNetworkAsync(virtualNetwork.Id, resource.Id);
        }
        catch (Exception ex)
        {
            if (resource is WebApplicationResource)
            {
                logger.LogWarning(
                    "Failed to connect web application resource {ResourceId} to virtual network {VirtualNetworkId}. This might be due to the web application not being ready yet",
                    resource.Id,
                    virtualNetwork.Id);
                logger.LogWarning(ex, ex.Message);
            }
            else
            {
                logger.LogError(ex, "Failed to connect resource {ResourceId} to virtual network {VirtualNetworkId}",
                    resource.Id, virtualNetwork.Id);
                throw new BadRequestException("Failed to connect resource to virtual network. Please try again later.");
            }
        }

        await baseResourceRepository.LogResourceAction(new AuditLogEntry
        {
            ActionName = nameof(ConnectVirtualNetworkCommand),
            ActionDisplayText = $"Connected resource {resource.Name} to virtual network {virtualNetwork.Name}",
            ResourceId = virtualNetwork.Id
        });
    }
}
