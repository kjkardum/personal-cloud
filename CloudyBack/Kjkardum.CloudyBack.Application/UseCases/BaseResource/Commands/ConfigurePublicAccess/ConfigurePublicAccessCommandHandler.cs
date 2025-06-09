using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Commands.ConfigurePublicAccess;

public class ConfigurePublicAccessCommandHandler(
    IReverseProxyClient reverseProxyClient,
    IKeyValueRepository keyValueRepository) : IRequestHandler<ConfigurePublicAccessCommand>
{
    public async Task Handle(ConfigurePublicAccessCommand request, CancellationToken cancellationToken)
    {
        var urlForHost = request.Host.Trim().TrimEnd('/');
        urlForHost = urlForHost.Contains("://") ? urlForHost : ("http://" + urlForHost);
        var uriObject = new Uri(urlForHost);
        var actualHostnameToUse = uriObject.Host;
        const int portToUse = 8080;
        var grafanaConnectionKeyValue = await keyValueRepository.GetValueAsync(
            KeyValueTableEntry.PublicHttpsAccessConnection);
        if (grafanaConnectionKeyValue != null)
        {
            var existingUrl = new Uri(grafanaConnectionKeyValue);
            if (existingUrl.Host == actualHostnameToUse
                && existingUrl.Port == portToUse &&
                existingUrl.Scheme == "https")
            {
                // No changes needed, already configured
                return;
            }
            await reverseProxyClient.RemoveProxyConfigurationByContainerName(
                "cloudyadminapp",
                existingUrl.Port,
                existingUrl.Host,
                existingUrl.Scheme == "https");
        }
        var newGrafanaUrl = $"https://{actualHostnameToUse}:{portToUse}";
        await keyValueRepository.SetValueAsync(KeyValueTableEntry.PublicHttpsAccessConnection, newGrafanaUrl);
        await reverseProxyClient.AddProxyConfigurationByContainerName(
            "cloudyadminapp",
            "personal-cloud_cloudyadminnetwork",
            portToUse,
            actualHostnameToUse,
            true);
    }
}
