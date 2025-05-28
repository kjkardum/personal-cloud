using AutoMapper;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Dto;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Queries.GetAll;

public class GetAllResourceGroupsQueryHandler(
    IResourceGroupRepository repository,
    IMapper mapper) : IRequestHandler<GetAllResourceGroupsQuery, PaginatedResponse<ResourceGroupSimpleDto>>
{
    public async Task<PaginatedResponse<ResourceGroupSimpleDto>> Handle(
        GetAllResourceGroupsQuery request,
        CancellationToken cancellationToken)
    {
        var (resourceGroups, totalCount) = await repository.GetPaginated(request.Pagination);

        var mappedResourceGroups = mapper.Map<IEnumerable<ResourceGroupSimpleDto>>(resourceGroups);

        return new PaginatedResponse<ResourceGroupSimpleDto>
        {
            Data = mappedResourceGroups,
            Page = request.Pagination.Page,
            PageSize = request.Pagination.PageSize,
            TotalCount = totalCount
        };
    }
}
