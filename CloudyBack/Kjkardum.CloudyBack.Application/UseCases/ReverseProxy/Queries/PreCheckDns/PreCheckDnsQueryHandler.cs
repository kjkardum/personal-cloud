using Kjkardum.CloudyBack.Application.UseCases.ReverseProxy.Dto;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace Kjkardum.CloudyBack.Application.UseCases.ReverseProxy.Queries.PreCheckDns;

public class PreCheckDnsQueryHandler(ILogger<PreCheckDnsQueryHandler> logger) : IRequestHandler<PreCheckDnsQuery, DnsCheckDto>
{
    public async Task<DnsCheckDto> Handle(PreCheckDnsQuery request, CancellationToken cancellationToken)
    {
        var urlToResolve = new Uri(request.Url.Contains("://") ? request.Url.Trim() : $"http://{request.Url.Trim()}");
        var hostnameToResolve = urlToResolve.Host;
        var ipBehindHostname = new List<string>();
        try
        {
            var hostEntryResolved = await Dns.GetHostEntryAsync(hostnameToResolve, cancellationToken);
            ipBehindHostname = hostEntryResolved.AddressList.Select(t => t.ToString()).ToList();
        } catch (SocketException)
        {
            logger.LogWarning("Failed to resolve hostname {Hostname} for URL {Url}", hostnameToResolve, request.Url);
        }
        var adminUrlToResolve = new Uri(request.AdminUrl);
        var adminHostnameToResolve = adminUrlToResolve.Host;
        var adminHostEntryResolved = await Dns.GetHostEntryAsync(adminHostnameToResolve, cancellationToken);
        var ipBehindAdminHostname = adminHostEntryResolved.AddressList.Select(t => t.ToString()).ToList();
        return new DnsCheckDto
        {
            Hostname = hostnameToResolve,
            IpsBehindHostname = ipBehindHostname,
            AdminHostname = adminHostnameToResolve,
            IpsBehindAdminHostname = ipBehindAdminHostname
        };
    }
}
