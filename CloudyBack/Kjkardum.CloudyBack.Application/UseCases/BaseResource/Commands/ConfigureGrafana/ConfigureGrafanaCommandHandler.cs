using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Commands.ConfigureGrafana;

public class ConfigureGrafanaCommandHandler(
    IObservabilityClient observabilityClient,
    IReverseProxyClient reverseProxyClient,
    IKeyValueRepository keyValueRepository) : IRequestHandler<ConfigureGrafanaCommand>
{
    public async Task Handle(ConfigureGrafanaCommand request, CancellationToken cancellationToken)
    {
        var urlForHost = request.Host.Trim().TrimEnd('/');
        urlForHost = urlForHost.Contains("://") ? urlForHost : ("http://" + urlForHost);
        var uriObject = new Uri(urlForHost);
        var actualHostnameToUse = uriObject.Host;
        var actualHttpsValueToUse = request.UseHttps;
        const int portToUse = 3000;
        var grafanaConnectionKeyValue = await keyValueRepository.GetValueAsync(
            KeyValueTableEntry.GrafanaPublicConnection);
        if (grafanaConnectionKeyValue != null)
        {
            var existingUri = new Uri(grafanaConnectionKeyValue);
            if (existingUri.Host == actualHostnameToUse
                && existingUri.Port == portToUse &&
                existingUri.Scheme == (actualHttpsValueToUse ? "https" : "http"))
            {
                // No changes needed, already configured
                return;
            }
            await reverseProxyClient.RemoveProxyConfigurationByContainerName(
                "cloudygrafanacontainer",
                existingUri.Port,
                existingUri.Host,
                existingUri.Scheme == "https");
        }

        await observabilityClient.CreateOrRunGrafana();
        var newGrafanaUrl = $"{(actualHttpsValueToUse ? "https" : "http")}://{actualHostnameToUse}:{portToUse}";
        await keyValueRepository.SetValueAsync(KeyValueTableEntry.GrafanaPublicConnection, newGrafanaUrl);
        await reverseProxyClient.AddProxyConfigurationByContainerName(
            "cloudygrafanacontainer",
            "cloudyobservabilitynetwork",
            portToUse,
            actualHostnameToUse,
            actualHttpsValueToUse);
    }
}
