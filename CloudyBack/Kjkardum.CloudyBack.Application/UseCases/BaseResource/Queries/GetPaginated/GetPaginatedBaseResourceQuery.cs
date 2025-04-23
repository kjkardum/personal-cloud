using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetPaginated;

public record GetPaginatedBaseResourceQuery(PaginatedRequest request) : IRequest<PaginatedResponse<BaseResourceDto>>
{
    public PaginatedRequest Pagination { get; } = request;
}
