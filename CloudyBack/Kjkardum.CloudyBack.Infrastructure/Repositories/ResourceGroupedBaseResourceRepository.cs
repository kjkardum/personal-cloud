using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using Kjkardum.CloudyBack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Kjkardum.CloudyBack.Infrastructure.Repositories;

public class ResourceGroupedBaseResourceRepository(ApplicationDbContext dbContext)
    : IResourceGroupedBaseResourceRepository
{
    public async Task<ResourceGroupedBaseResource?> GetByIdAsync(Guid id) =>
        await dbContext.ResourceGroupedBaseResources
            .Include(r => r.ResourceGroup)
            .FirstOrDefaultAsync(r => r.Id == id);
}
