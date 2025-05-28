using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using Kjkardum.CloudyBack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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
}
