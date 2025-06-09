using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Commands.ConfigurePublicAccess;

public class ConfigurePublicAccessCommand: IRequest
{
    public string Host { get; set; }
}
