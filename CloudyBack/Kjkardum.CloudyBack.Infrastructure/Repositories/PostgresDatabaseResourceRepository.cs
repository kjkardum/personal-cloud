using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using Kjkardum.CloudyBack.Infrastructure.Persistence;

namespace Kjkardum.CloudyBack.Infrastructure.Repositories;

public class PostgresDatabaseResourceRepository(ApplicationDbContext dbContext): IPostgresDatabaseResourceRepository
{
    public async Task<PostgresDatabaseResource> Create(PostgresDatabaseResource postgresDatabaseResource)
    {
        await dbContext.PostgresDatabaseResources.AddAsync(postgresDatabaseResource);
        await dbContext.SaveChangesAsync();
        return postgresDatabaseResource;
    }
}
