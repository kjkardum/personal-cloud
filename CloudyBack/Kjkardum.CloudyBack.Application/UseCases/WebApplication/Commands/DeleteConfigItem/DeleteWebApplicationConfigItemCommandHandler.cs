using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.WebApplication.Commands.DeleteConfigItem;

public class DeleteWebApplicationConfigItemCommandHandler(
    IWebApplicationResourceRepository webApplicationResourceRepository,
    IBaseResourceRepository baseResourceRepository) : IRequestHandler<DeleteWebApplicationConfigItemCommand>
{
    public async Task Handle(DeleteWebApplicationConfigItemCommand request, CancellationToken cancellationToken)
    {
        await webApplicationResourceRepository.DeleteConfigurationEntry(new WebApplicationConfigurationEntry
            {
                WebApplicationResourceId = request.WebApplicationId,
                Key = request.Key
            });
        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(DeleteWebApplicationConfigItemCommand),
                ActionDisplayText
                    = $"Delete configuration item {request.Key} for Web application {request.WebApplicationId}",
                ResourceId = request.WebApplicationId
            });
    }
}
