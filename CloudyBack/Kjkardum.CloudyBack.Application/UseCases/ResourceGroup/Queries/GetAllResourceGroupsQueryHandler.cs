using AutoMapper;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Dto;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Queries;

public class GetAllResourceGroupsQueryHandler(
    IResourceGroupRepository repository,
    IMapper mapper) : IRequestHandler<GetAllResourceGroupsQuery, PaginatedResponse<ResourceGroupDto>>
{
    public async Task<PaginatedResponse<ResourceGroupDto>> Handle(
        GetAllResourceGroupsQuery request,
        CancellationToken cancellationToken)
    {
        var (resourceGroups, totalCount) = await repository.GetPaginated(request.Pagination);

        var mappedResourceGroups = mapper.Map<IEnumerable<ResourceGroupDto>>(resourceGroups);

        return new PaginatedResponse<ResourceGroupDto>
        {
            Data = mappedResourceGroups,
            Page = request.Pagination.Page,
            PageSize = request.Pagination.PageSize,
            TotalCount = totalCount
        };
    }
}
