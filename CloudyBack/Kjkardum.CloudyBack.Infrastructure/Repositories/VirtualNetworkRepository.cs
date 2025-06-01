using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Domain.Entities;
using Kjkardum.CloudyBack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Kjkardum.CloudyBack.Infrastructure.Repositories;

public class VirtualNetworkRepository(ApplicationDbContext dbContext): IVirtualNetworkRepository
{
    public async Task<VirtualNetworkResource?> GetById(Guid id)
    {
        var result = await dbContext.VirtualNetworkResources
            .Include(t => t.ResourceGroup)
            .Include(t => t.NetworkConnections!)
            .ThenInclude(t => t.Resource)
            .FirstOrDefaultAsync(x => x.Id == id);

        return result;
    }

    public async Task AttachVirtualNetwork(Guid virtualNetworkId, Guid resourceId)
    {
        var existingConnection = await dbContext.VirtualNetworkConnections
            .FirstOrDefaultAsync(x => x.VirtualNetworkId == virtualNetworkId && x.ResourceId == resourceId);
        if (existingConnection != null)
        {
            // If the connection already exists, we can skip creating a new one.
            return;
        }
        var connection = new VirtualNetworkConnection
        {
            VirtualNetworkId = virtualNetworkId,
            ResourceId = resourceId
        };
        await dbContext.VirtualNetworkConnections.AddAsync(connection);
        await dbContext.SaveChangesAsync();
    }

    public async Task<(IEnumerable<VirtualNetworkResource>, int)> GetPaginated(PaginatedRequest pagination)
    {
        var query = dbContext.VirtualNetworkResources.AsQueryable();
        if (!string.IsNullOrEmpty(pagination.FilterBy))
        {
            query = query.Where(x => x.Name.Contains(pagination.FilterBy));
        }
        var orderBy = (pagination.OrderBy ?? string.Empty).Split(',');
        if (orderBy.Length == 2)
        {
            Expression<Func<VirtualNetworkResource, object?>> orderByFunc = orderBy[0].ToLowerInvariant() switch
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

        var resourceGroups = await query.ToListAsync();
        return (resourceGroups, total);
    }

    public async Task<VirtualNetworkResource> Create(VirtualNetworkResource virtualNetworkResource)
    {
        await dbContext.VirtualNetworkResources.AddAsync(virtualNetworkResource);
        await dbContext.SaveChangesAsync();
        return virtualNetworkResource;
    }
}
