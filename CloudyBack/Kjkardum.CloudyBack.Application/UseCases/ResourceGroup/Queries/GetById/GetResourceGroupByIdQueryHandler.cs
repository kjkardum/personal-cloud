using ApiExceptions.Exceptions;
using AutoMapper;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Dto;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Queries.GetById;

public class GetResourceGroupByIdQueryHandler(
    IResourceGroupRepository resourceGroupRepository,
    IMapper mapper) : IRequestHandler<GetResourceGroupByIdQuery, ResourceGroupDto>
{
    public async Task<ResourceGroupDto> Handle(GetResourceGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var resourceGroup = await resourceGroupRepository.GetById(request.Id);

        if (resourceGroup == null)
        {
            throw new EntityNotFoundException($"Resource group with ID {request.Id} not found.");
        }

        return mapper.Map<ResourceGroupDto>(resourceGroup);
    }
}
