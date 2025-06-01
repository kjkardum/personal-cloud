using AutoMapper;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.VirtualNetwork.Queries.GetPaginated;

public class GetPaginatedVirtualNetworksQueryHandler(
    IVirtualNetworkRepository virtualNetworkRepository,
    IMapper mapper) : IRequestHandler<GetPaginatedVirtualNetworksQuery, PaginatedResponse<VirtualNetworkSimpleDto>>
{
    public async Task<PaginatedResponse<VirtualNetworkSimpleDto>> Handle(
        GetPaginatedVirtualNetworksQuery request,
        CancellationToken cancellationToken)
    {
        var (resourceGroups, totalCount) = await virtualNetworkRepository.GetPaginated(request.Pagination);

        var mappedResourceGroups = mapper.Map<IEnumerable<VirtualNetworkSimpleDto>>(resourceGroups);

        return new PaginatedResponse<VirtualNetworkSimpleDto>
        {
            Data = mappedResourceGroups,
            Page = request.Pagination.Page,
            PageSize = request.Pagination.PageSize,
            TotalCount = totalCount
        };
    }
}
