using AutoMapper;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Dto;
using Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Queries;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetPaginated;

public class GetPaginatedBaseResourceQueryHandler(
    IBaseResourceRepository baseResourceRepository,
    IMapper mapper) : IRequestHandler<GetPaginatedBaseResourceQuery, PaginatedResponse<BaseResourceDto>>
{
    public async Task<PaginatedResponse<BaseResourceDto>> Handle(
        GetPaginatedBaseResourceQuery request,
        CancellationToken cancellationToken)
    {
        var (baseResources, totalCount) = await baseResourceRepository.GetPaginated(request.Pagination);

        var mappedBaseResource = baseResources.Select(mapper.Map<BaseResourceDto>).ToList();

        return new PaginatedResponse<BaseResourceDto>
        {
            Data = mappedBaseResource,
            Page = request.Pagination.Page,
            PageSize = request.Pagination.PageSize,
            TotalCount = totalCount
        };
    }
}
