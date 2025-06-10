using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Domain.Entities;
using Kjkardum.CloudyBack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Kjkardum.CloudyBack.Infrastructure.Repositories;

public class BaseResourceRepository(ApplicationDbContext dbContext) : IBaseResourceRepository
{
    public async Task<BaseResource?> GetById(Guid id) =>
        await dbContext.Resources
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<(IEnumerable<BaseResource>, int)> GetPaginated(PaginatedRequest pagination, string resourceType)
    {
        var query = dbContext.Resources.AsQueryable();
        if (!string.IsNullOrEmpty(pagination.FilterBy))
        {
            query = query.Where(x => x.Name.Contains(pagination.FilterBy));
        }
        if (!string.IsNullOrEmpty(resourceType))
        {
            if (typeof(WebApplicationResource).ToString()
                .EndsWith(resourceType, StringComparison.InvariantCultureIgnoreCase))
            {
                query = query.OfType<WebApplicationResource>();
            }
            else if (typeof(PostgresDatabaseResource).ToString()
                .EndsWith(resourceType, StringComparison.InvariantCultureIgnoreCase))
            {
                query = query.OfType<PostgresDatabaseResource>();
            }
            else if (typeof(PostgresServerResource).ToString()
                .EndsWith(resourceType, StringComparison.InvariantCultureIgnoreCase))
            {
                query = query.OfType<PostgresServerResource>();
            }
            else if (typeof(VirtualNetworkResource).ToString()
                .EndsWith(resourceType, StringComparison.InvariantCultureIgnoreCase))
            {
                query = query.OfType<VirtualNetworkResource>();
            }
            else if (typeof(KafkaClusterResource).ToString()
                .EndsWith(resourceType, StringComparison.InvariantCultureIgnoreCase))
            {
                query = query.OfType<KafkaClusterResource>();
            }
            else if (typeof(ResourceGroup).ToString()
                .EndsWith(resourceType, StringComparison.InvariantCultureIgnoreCase))
            {
                query = query.OfType<ResourceGroup>();
            }
        }

        var orderBy = (pagination.OrderBy ?? string.Empty).Split(',');
        if (orderBy.Length == 2)
        {
            Expression<Func<BaseResource, object?>> orderByFunc = orderBy[0].ToLowerInvariant() switch
            {
                "name" => x => x.Name,
                "createdat" => x => x.CreatedAt,
                "updatedat" => x => x.UpdatedAt,
                _ => x => x.Name
            };

            query = orderBy[1].ToLowerInvariant() switch
            {
                "desc" => query.OrderByDescending(orderByFunc),
                _ => query.OrderBy(orderByFunc)
            };
        }

        var total = await query.CountAsync();

        query = query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize);

        var baseResource = await query.ToListAsync();
        return (baseResource, total);
    }

    public async Task<(IEnumerable<AuditLogEntry>, int)> GetAuditLogEntries(
        Guid resourceId,
        PaginatedRequest pagination) => resourceId != Guid.Empty
        ? await InnerGetAuditLogEntries(resourceId, pagination)
        : throw new ArgumentOutOfRangeException(nameof(resourceId));

    public async Task<(IEnumerable<AuditLogEntry>, int)> GetGlobalAuditLogEntries(PaginatedRequest pagination)
        => await InnerGetAuditLogEntries(Guid.Empty, pagination);

    public async Task<(IEnumerable<AuditLogEntry>, int)> InnerGetAuditLogEntries(
        Guid resourceId,
        PaginatedRequest pagination)
    {
        var query = dbContext.AuditLogEntries.AsQueryable();
        if (resourceId != Guid.Empty)
        {
            query = query.Where(x => x.ResourceId == resourceId);
        }

        if (!string.IsNullOrWhiteSpace(pagination.FilterBy))
        {
            query = query.Where(x => x.ActionName.Contains(pagination.FilterBy) ||
                x.ActionDisplayText.Contains(pagination.FilterBy) ||
                (x.ActionMetadata != null && x.ActionMetadata.Contains(pagination.FilterBy)));
        }

        var total = await query.CountAsync();

        query = query.OrderByDescending(t => t.Timestamp);

        query = query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize);

        var auditLogEntries = await query.ToListAsync();
        return (auditLogEntries, total);
    }

    public async Task LogResourceAction(AuditLogEntry auditLogEntry)
    {
        await dbContext.AuditLogEntries.AddAsync(auditLogEntry);
        await dbContext.SaveChangesAsync();
    }
}
