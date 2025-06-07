using Kjkardum.CloudyBack.Application.UseCases.ReverseProxy.Dto;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.ReverseProxy.Queries.PreCheckDns;

public class PreCheckDnsQuery : IRequest<DnsCheckDto>
{
    public string Url { get; set; }
    public string AdminUrl { get; set; }
}
