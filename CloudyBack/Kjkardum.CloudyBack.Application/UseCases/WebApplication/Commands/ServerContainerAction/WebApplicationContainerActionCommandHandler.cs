using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.WebApplication.Commands.ServerContainerAction;

internal class WebApplicationContainerActionCommandHandler(
    IBaseResourceRepository baseResourceRepository,
    IWebApplicationClient webApplicationClient) : IRequestHandler<WebApplicationContainerActionCommand>
{
    public async Task Handle(WebApplicationContainerActionCommand request, CancellationToken cancellationToken)
    {
        var resource = await baseResourceRepository.GetById(request.Id);
        if (resource is not WebApplicationResource)
        {
            throw new EntityNotFoundException($"Web application with id {request.Id} not found.");
        }
        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(WebApplicationContainerActionCommand),
                ActionDisplayText = $"Trigger docker action {request.ActionId} on server",
                ResourceId = request.Id
            });
        switch (request.ActionId)
        {
            case "start":
                await webApplicationClient.StartServerAsync(request.Id);
                break;
            case "stop":
                await webApplicationClient.StopServerAsync(request.Id);
                break;
            case "restart":
                await webApplicationClient.RestartServerAsync(request.Id);
                break;
            default:
                throw new EntityNotFoundException("Invalid action id");
        }
    }
}
