using AutoMapper;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.WebApplication.Dto;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.WebApplication.Queries.GetPaginated;

public class GetPaginatedWebApplicationResourceQueryHandler(
    IWebApplicationResourceRepository repository,
    IMapper mapper)
    : IRequestHandler<GetPaginatedWebApplicationResourceQuery, PaginatedResponse<WebApplicationResourceDto>>
{
    public async Task<PaginatedResponse<WebApplicationResourceDto>> Handle(
        GetPaginatedWebApplicationResourceQuery request,
        CancellationToken cancellationToken)
    {
        var (baseResources, totalCount) = await repository.GetPaginated(request.Pagination);

        var mappedBaseResource = baseResources.Select(mapper.Map<WebApplicationResourceDto>).ToList();

        return new PaginatedResponse<WebApplicationResourceDto>
        {
            Data = mappedBaseResource,
            Page = request.Pagination.Page,
            PageSize = request.Pagination.PageSize,
            TotalCount = totalCount
        };
    }
}
