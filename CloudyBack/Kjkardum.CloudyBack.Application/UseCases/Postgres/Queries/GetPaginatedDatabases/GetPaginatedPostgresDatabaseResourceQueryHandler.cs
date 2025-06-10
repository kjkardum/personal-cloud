using AutoMapper;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Queries.GetPaginatedDatabases;

public class GetPaginatedPostgresDatabaseResourceQueryHandler(
    IPostgresDatabaseResourceRepository postgresDatabaseResourceRepository,
    IMapper mapper)
    : IRequestHandler<GetPaginatedPostgresDatabaseResourceQuery, PaginatedResponse<PostgresDatabaseResourceDto>>
{
    public async Task<PaginatedResponse<PostgresDatabaseResourceDto>> Handle(
        GetPaginatedPostgresDatabaseResourceQuery request,
        CancellationToken cancellationToken)
    {
        var (postgresDatabases, totalCount) = await postgresDatabaseResourceRepository.GetPaginated(request.Pagination);

        var mappedBaseResource = postgresDatabases.Select(mapper.Map<PostgresDatabaseResourceDto>).ToList();

        return new PaginatedResponse<PostgresDatabaseResourceDto>
        {
            Data = mappedBaseResource,
            Page = request.Pagination.Page,
            PageSize = request.Pagination.PageSize,
            TotalCount = totalCount
        };
    }
}
