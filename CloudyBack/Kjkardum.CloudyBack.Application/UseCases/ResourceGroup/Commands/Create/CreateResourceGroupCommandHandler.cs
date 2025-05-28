using AutoMapper;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Dto;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Commands.Create;

public class CreateResourceGroupCommandHandler(
    IResourceGroupRepository repository,
    IBaseResourceRepository baseResourceRepository,
    IMapper mapper) : IRequestHandler<CreateResourceGroupCommand, ResourceGroupSimpleDto>
{
    public async Task<ResourceGroupSimpleDto> Handle(
        CreateResourceGroupCommand request,
        CancellationToken cancellationToken)
    {
        var resourceGroup = new Domain.Entities.ResourceGroup
        {
            Name = request.Name,
        };

        var createdResourceGroup = await repository.Create(resourceGroup);

        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(CreateResourceGroupCommand),
                ActionDisplayText = $"Create new resource group {request.Name}",
                ResourceId = createdResourceGroup.Id
            });

        var mappedResourceGroup = mapper.Map<ResourceGroupSimpleDto>(createdResourceGroup);

        return mappedResourceGroup;
    }
}
