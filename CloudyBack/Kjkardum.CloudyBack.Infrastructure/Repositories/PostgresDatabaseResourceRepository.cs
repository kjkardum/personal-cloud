using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Domain.Entities;
using Kjkardum.CloudyBack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Kjkardum.CloudyBack.Infrastructure.Repositories;

public class PostgresDatabaseResourceRepository(ApplicationDbContext dbContext): IPostgresDatabaseResourceRepository
{
    public async Task<PostgresDatabaseResource> Create(PostgresDatabaseResource postgresDatabaseResource)
    {
        await dbContext.PostgresDatabaseResources.AddAsync(postgresDatabaseResource);
        await dbContext.SaveChangesAsync();
        return postgresDatabaseResource;
    }

    public async Task<PostgresDatabaseResource?> GetById(Guid id)
    {
        var postgresDatabaseResource = await dbContext.PostgresDatabaseResources
            .Include(t => t.PostgresDatabaseServerResource)
            .Include(t => t.ResourceGroup)
            .FirstOrDefaultAsync(x => x.Id == id);

        return postgresDatabaseResource;
    }

    public async Task<(IEnumerable<PostgresDatabaseResource> databases, int totalCount)> GetPaginated(
        PaginatedRequest pagination)
    {
        var query = dbContext.PostgresDatabaseResources.AsQueryable();
        if (!string.IsNullOrEmpty(pagination.FilterBy))
        {
            query = query.Where(x => x.Name.Contains(pagination.FilterBy));
        }
        var orderBy = (pagination.OrderBy ?? string.Empty).Split(',');
        if (orderBy.Length == 2)
        {
            Expression<Func<PostgresDatabaseResource, object?>> orderByFunc = orderBy[0].ToLowerInvariant() switch
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

        var databases = await query
            .Include(t => t.PostgresDatabaseServerResource)
            .Include(t => t.ResourceGroup)
            .ToListAsync();
        return (databases, total);
    }
}
