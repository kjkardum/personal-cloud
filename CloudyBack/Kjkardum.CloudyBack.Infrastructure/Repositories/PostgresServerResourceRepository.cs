using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Domain.Entities;
using Kjkardum.CloudyBack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Kjkardum.CloudyBack.Infrastructure.Repositories;

public class PostgresServerResourceRepository(ApplicationDbContext dbContext): IPostgresServerResourceRepository
{
    public async Task<PostgresServerResource> Create(PostgresServerResource postgresServerResource)
    {
        await dbContext.PostgresServerResources.AddAsync(postgresServerResource);
        await dbContext.SaveChangesAsync();
        return postgresServerResource;
    }

    public async Task<PostgresServerResource?> GetById(Guid id)
    {
        var postgresServerResource = await dbContext.PostgresServerResources
            .Include(t => t.ResourceGroup)
            .Include(t => t.PostgresDatabaseResources)
            .FirstOrDefaultAsync(x => x.Id == id);
        return postgresServerResource;
    }

    public async Task<(IEnumerable<PostgresServerResource>, int)> GetPaginated(PaginatedRequest pagination)
    {
        //page (from 1), pageSize, filterBy (looks at name), orderBy, where orderBys are allowed to be: Name, CreatedAt, UpdatedAt in a format Name,Asc or Name,Desc
        var query = dbContext.PostgresServerResources.AsQueryable();
        if (!string.IsNullOrEmpty(pagination.FilterBy))
        {
            query = query.Where(x => x.Name.Contains(pagination.FilterBy));
        }
        var orderBy = (pagination.OrderBy ?? string.Empty).Split(',');
        if (orderBy.Length == 2)
        {
            Expression<Func<PostgresServerResource, object?>> orderByFunc = orderBy[0].ToLowerInvariant() switch
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

        var postgresServerResources = await query.ToListAsync();
        var total = await query.CountAsync();
        return (postgresServerResources, total);
    }

    public async Task Delete(PostgresServerResource postgresServerResource)
    {
        dbContext.PostgresServerResources.Remove(postgresServerResource);
        await dbContext.SaveChangesAsync();
    }
}
