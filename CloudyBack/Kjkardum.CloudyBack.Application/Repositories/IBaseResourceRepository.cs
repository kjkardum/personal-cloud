using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Repositories;

public interface IBaseResourceRepository
{
    Task<BaseResource?> GetById(Guid id);
    Task<(IEnumerable<BaseResource>, int)> GetPaginated(PaginatedRequest pagination);
    Task<(IEnumerable<AuditLogEntry>, int)> GetAuditLogEntries(Guid resourceId, PaginatedRequest pagination);
    Task<(IEnumerable<AuditLogEntry>, int)> GetGlobalAuditLogEntries(PaginatedRequest pagination);
    Task LogResourceAction(AuditLogEntry auditLogEntry);
}
