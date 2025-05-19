using ApiExceptions.Exceptions;
using AutoMapper;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetResourceGrouped;

public class GetResourceGroupedBaseResourceQueryHandler(
    IResourceGroupedBaseResourceRepository repository,
    IMapper mapper)
    : IRequestHandler<GetResourceGroupedBaseResourceQuery, ResourceGroupedBaseResourceDto>
{
    public async Task<ResourceGroupedBaseResourceDto> Handle(
        GetResourceGroupedBaseResourceQuery request,
        CancellationToken cancellationToken)
    {
        var resource = await repository.GetByIdAsync(request.Id);
        if (resource is null)
        {
            throw new EntityNotFoundException($"Resource with id {request.Id} not found.");
        }
        var resourceDto = mapper.Map<ResourceGroupedBaseResourceDto>(resource);
        return resourceDto;
    }
}
