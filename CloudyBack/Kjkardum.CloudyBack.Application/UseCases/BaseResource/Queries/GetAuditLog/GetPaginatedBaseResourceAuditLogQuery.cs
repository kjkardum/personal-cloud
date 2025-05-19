using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetAuditLog;

public class GetPaginatedBaseResourceAuditLogQuery(Guid id, PaginatedRequest request): IRequest<PaginatedResponse<AuditLogDto>>
{
    public PaginatedRequest Pagination { get; } = request;
    public Guid ResourceId { get; } = id;
}
