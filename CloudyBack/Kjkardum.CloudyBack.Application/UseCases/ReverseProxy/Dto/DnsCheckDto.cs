namespace Kjkardum.CloudyBack.Application.UseCases.ReverseProxy.Dto;

public class DnsCheckDto
{
    public string Hostname { get; set; }
    public List<string> IpsBehindHostname { get; set; }
    public string AdminHostname { get; set; }
    public List<string> IpsBehindAdminHostname { get; set; }
}
