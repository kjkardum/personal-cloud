using AutoMapper;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Dto;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Commands.Create;

public class CreateResourceGroupCommandHandler(
    IResourceGroupRepository repository,
    IMapper mapper) : IRequestHandler<CreateResourceGroupCommand, ResourceGroupDto>
{
    public async Task<ResourceGroupDto> Handle(
        CreateResourceGroupCommand request,
        CancellationToken cancellationToken)
    {
        var resourceGroup = new Domain.Entities.ResourceGroup
        {
            Name = request.Name,
        };

        var createdResourceGroup = await repository.Create(resourceGroup);

        var mappedResourceGroup = mapper.Map<ResourceGroupDto>(createdResourceGroup);

        return mappedResourceGroup;
    }
}
