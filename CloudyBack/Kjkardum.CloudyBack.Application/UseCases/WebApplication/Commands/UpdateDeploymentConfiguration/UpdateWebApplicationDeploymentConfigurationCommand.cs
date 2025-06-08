using Kjkardum.CloudyBack.Domain.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace Kjkardum.CloudyBack.Application.UseCases.WebApplication.Commands.UpdateDeploymentConfiguration;

public class UpdateWebApplicationDeploymentConfigurationCommand: IRequest
{
    [JsonIgnore] public Guid Id { get; set; }
    public string BuildCommand { get; set; } = string.Empty;
    public string StartupCommand { get; set; } = string.Empty;
    public WebApplicationRuntimeType? RuntimeType { get; set; }
    public int Port { get; set; }
}
