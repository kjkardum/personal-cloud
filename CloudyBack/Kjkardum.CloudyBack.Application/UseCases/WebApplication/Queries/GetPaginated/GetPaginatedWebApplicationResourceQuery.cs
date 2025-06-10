using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.WebApplication.Dto;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.WebApplication.Queries.GetPaginated;

public record GetPaginatedWebApplicationResourceQuery(
    PaginatedRequest paginatedRequest): IRequest<PaginatedResponse<WebApplicationResourceDto>>
{
    public PaginatedRequest Pagination { get; init; } = paginatedRequest;
}
