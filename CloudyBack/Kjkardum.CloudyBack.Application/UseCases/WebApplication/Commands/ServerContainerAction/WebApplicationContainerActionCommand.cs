using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.WebApplication.Commands.ServerContainerAction;

public class WebApplicationContainerActionCommand: IRequest
{
    public Guid Id { get; set; }
    public string ActionId { get; set; }
}
