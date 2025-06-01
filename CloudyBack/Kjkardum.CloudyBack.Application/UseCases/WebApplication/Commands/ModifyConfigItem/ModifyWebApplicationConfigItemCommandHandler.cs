using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.WebApplication.Commands.ModifyConfigItem;

public class ModifyWebApplicationConfigItemCommandHandler(
    IWebApplicationResourceRepository webApplicationResourceRepository,
    IBaseResourceRepository baseResourceRepository) : IRequestHandler<ModifyWebApplicationConfigItemCommand>
{
    public async Task Handle(ModifyWebApplicationConfigItemCommand request, CancellationToken cancellationToken)
    {
        await webApplicationResourceRepository.UpsertConfigurationEntry(new WebApplicationConfigurationEntry
            {
                Key = request.Key,
                Value = request.Value,
                WebApplicationResourceId = request.WebApplicationId
            });
        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(ModifyWebApplicationConfigItemCommand),
                ActionDisplayText
                    = $"Modify configuration item {request.Key} for Web application {request.WebApplicationId}",
                ResourceId = request.WebApplicationId
            });
    }
}
