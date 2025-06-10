using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Queries.GetPaginatedDatabases;

public record GetPaginatedPostgresDatabaseResourceQuery(PaginatedRequest request)
    : IRequest<PaginatedResponse<PostgresDatabaseResourceDto>>
{
    public PaginatedRequest Pagination { get; } = request;
}
