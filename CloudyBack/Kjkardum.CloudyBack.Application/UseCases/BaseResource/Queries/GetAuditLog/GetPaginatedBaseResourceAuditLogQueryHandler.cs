using AutoMapper;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetAuditLog;

public class GetPaginatedBaseResourceAuditLogQueryHandler(
    IBaseResourceRepository baseResourceRepository,
    IMapper mapper)
    : IRequestHandler<GetPaginatedBaseResourceAuditLogQuery, PaginatedResponse<AuditLogDto>>
{
    public async Task<PaginatedResponse<AuditLogDto>> Handle(
        GetPaginatedBaseResourceAuditLogQuery request,
        CancellationToken cancellationToken)
    {
        IEnumerable<AuditLogEntry> logs;
        int count;

        if (request.ResourceId == Guid.Empty)
        {
            (logs, count) = await baseResourceRepository.GetGlobalAuditLogEntries(
                request.Pagination);
        }
        else
        {
            (logs, count) = await baseResourceRepository.GetAuditLogEntries(
                request.ResourceId,
                request.Pagination);
        }

        var auditLogDtos = logs.Select(mapper.Map<AuditLogDto>);
        var paginatedResponse = new PaginatedResponse<AuditLogDto>
        {
            Data = auditLogDtos.ToList(),
            TotalCount = count,
            Page = request.Pagination.Page,
            PageSize = request.Pagination.PageSize
        };

        return paginatedResponse;
    }
}
