using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Domain.Entities;
using Kjkardum.CloudyBack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Kjkardum.CloudyBack.Infrastructure.Repositories;

public class ResourceGroupRepository(ApplicationDbContext dbContext): IResourceGroupRepository
{
    public async Task<ResourceGroup> Create(ResourceGroup resourceGroup)
    {
        await dbContext.ResourceGroups.AddAsync(resourceGroup);
        await dbContext.SaveChangesAsync();
        return resourceGroup;
    }

    public async Task<ResourceGroup?> GetById(Guid id)
    {
        var resourceGroup = await dbContext.ResourceGroups.FirstOrDefaultAsync(x => x.Id == id);
        return resourceGroup;
    }

    public async Task<(IEnumerable<ResourceGroup>, int)> GetPaginated(PaginatedRequest pagination)
    {
        //page (from 1), pageSize, filterBy (looks at name), orderBy, where orderBys are allowed to be: Name, CreatedAt, UpdatedAt in a format Name,Asc or Name,Desc
        var query = dbContext.ResourceGroups.AsQueryable();
        if (!string.IsNullOrEmpty(pagination.FilterBy))
        {
            query = query.Where(x => x.Name.Contains(pagination.FilterBy));
        }
        var orderBy = (pagination.OrderBy ?? string.Empty).Split(',');
        if (orderBy.Length == 2)
        {
            Expression<Func<ResourceGroup, object?>> orderByFunc = orderBy[0].ToLowerInvariant() switch
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

        query = query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize);

        var resourceGroups = await query.ToListAsync();
        var total = await query.CountAsync();
        return (resourceGroups, total);
    }
}
