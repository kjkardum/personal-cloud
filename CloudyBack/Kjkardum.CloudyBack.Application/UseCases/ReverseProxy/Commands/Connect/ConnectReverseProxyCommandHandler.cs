using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.ReverseProxy.Commands.Connect;

public class ConnectReverseProxyCommandHandler(
    IVirtualNetworkableBaseResourceRepository virtualNetworkableBaseResourceRepository,
    IReverseProxyClient reverseProxyClient,
    IBaseResourceRepository baseResourceRepository) : IRequestHandler<ConnectReverseProxyCommand>
{
    public async Task Handle(ConnectReverseProxyCommand request, CancellationToken cancellationToken)
    {
        var resource = await virtualNetworkableBaseResourceRepository.GetById(request.ResourceId);
        if (resource == null)
        {
            throw new EntityNotFoundException("Resource not found");
        }
        var urlForHost = request.UrlForHost.Trim().TrimEnd('/');
        urlForHost = urlForHost.Contains("://") ? urlForHost : ("http://" + urlForHost);
        var uriObject = new Uri(urlForHost);
        var actualHostnameToUse = uriObject.Host;
        var actualHttpsValueToUse = resource.IsHttpProtocol() && request.UseHttps;
        var portToUse = resource.GetProxyablePort();

        // Check if the resource is already connected
        if (resource.PublicProxyConfigurations!
            .Any(t => t.UseHttps == actualHttpsValueToUse && t.Domain == actualHostnameToUse && t.Port == portToUse))
        {
            throw new EntityAlreadyExistsException("Resource is already connected to a reverse proxy");
        }
        var newConfiguration = new PublicProxyConfiguration
        {
            Domain = actualHostnameToUse,
            Port = portToUse,
            UseHttps = actualHttpsValueToUse,
            VirtualNetworkableBaseResourceId = resource.Id
        };

        // Connect the resource to the reverse proxy
        await virtualNetworkableBaseResourceRepository.AddProxyConfiguration(newConfiguration);
        await reverseProxyClient.AddProxyConfiguration(
            resource.Id,
            portToUse,
            actualHostnameToUse,
            actualHttpsValueToUse);

        // Log audit event
        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(ConnectReverseProxyCommand),
                ActionDisplayText
                    = $"Connect {resource.Name} to public network at {actualHostnameToUse}:{portToUse}",
                ResourceId = resource.Id
            });
    }
}
