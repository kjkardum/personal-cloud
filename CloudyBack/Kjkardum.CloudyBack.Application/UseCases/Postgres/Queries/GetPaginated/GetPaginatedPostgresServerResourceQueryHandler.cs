using AutoMapper;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetPaginated;

public class GetPaginatedPostgresServerResourceQueryHandler(
    IPostgresServerResourceRepository postgresServerResourceRepository,
    IMapper mapper)
    : IRequestHandler<GetPaginatedPostgresServerResourceQuery, PaginatedResponse<PostgresServerResourceDto>>
{
    public async Task<PaginatedResponse<PostgresServerResourceDto>> Handle(
        GetPaginatedPostgresServerResourceQuery request,
        CancellationToken cancellationToken)
    {
        var (postgresServers, totalCount) = await postgresServerResourceRepository.GetPaginated(request.Pagination);

        var mappedBaseResource = postgresServers.Select(mapper.Map<PostgresServerResourceDto>).ToList();

        return new PaginatedResponse<PostgresServerResourceDto>
        {
            Data = mappedBaseResource,
            Page = request.Pagination.Page,
            PageSize = request.Pagination.PageSize,
            TotalCount = totalCount
        };
    }
}
