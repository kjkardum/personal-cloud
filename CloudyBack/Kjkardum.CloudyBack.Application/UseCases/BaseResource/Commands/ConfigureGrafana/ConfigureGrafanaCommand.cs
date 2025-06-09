using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Commands.ConfigureGrafana;

public class ConfigureGrafanaCommand: IRequest
{
    public string Host { get; set; }
    public bool UseHttps { get; set; }
}
