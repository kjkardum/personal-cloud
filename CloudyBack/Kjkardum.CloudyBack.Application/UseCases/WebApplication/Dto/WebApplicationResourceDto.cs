using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using Kjkardum.CloudyBack.Domain.Enums;

namespace Kjkardum.CloudyBack.Application.UseCases.WebApplication.Dto;

public class WebApplicationResourceDto: VirtualNetworkableBaseResourceDto
{
    public string SourcePath { get; set; }
    public WebApplicationSourceType SourceType { get; set; }
    public string BuildCommand { get; set; } = string.Empty;
    public string StartupCommand { get; set; } = string.Empty;
    public string HealthCheckUrl { get; set; } = string.Empty;
    public int Port { get; set; }
    public ICollection<WebApplicationConfigurationEntryDto>? Configuration { get; set; }
}
