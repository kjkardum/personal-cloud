using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.ReverseProxy.Commands.Disconnect;

public class DisconnectReverseProxyCommandHandler(
    IVirtualNetworkableBaseResourceRepository virtualNetworkableBaseResourceRepository,
    IReverseProxyClient reverseProxyClient,
    IBaseResourceRepository baseResourceRepository) : IRequestHandler<DisconnectReverseProxyCommand>
{
    public async Task Handle(DisconnectReverseProxyCommand request, CancellationToken cancellationToken)
    {
        var resource = await virtualNetworkableBaseResourceRepository.GetById(request.ResourceId);
        if (resource == null)
        {
            throw new EntityNotFoundException("Resource not found");
        }

        // Find the configuration to remove
        var publicConfig = resource.PublicProxyConfigurations!
            .FirstOrDefault(t => t.Id == request.ConnectionId);
        if (publicConfig == null)
        {
            throw new EntityNotFoundException("Proxy configuration not found");
        }

        // Remove the proxy configuration
        await virtualNetworkableBaseResourceRepository.RemoveProxyConfiguration(publicConfig.Id);
        await reverseProxyClient.RemoveProxyConfiguration(
            request.ResourceId,
            publicConfig.Port,
            publicConfig.Domain,
            publicConfig.UseHttps);

        // Log audit event
        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(DisconnectReverseProxyCommand),
                ActionDisplayText
                    = $"Disconnect {resource.Name} from public network at {publicConfig.Domain}:{publicConfig.Port}",
                ResourceId = resource.Id
            });
    }
}
