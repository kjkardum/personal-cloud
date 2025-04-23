using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetPaginated;

public record GetPaginatedPostgresServerResourceQuery(PaginatedRequest request)
    : IRequest<PaginatedResponse<PostgresServerResourceDto>>
{
    public PaginatedRequest Pagination { get; } = request;
}
