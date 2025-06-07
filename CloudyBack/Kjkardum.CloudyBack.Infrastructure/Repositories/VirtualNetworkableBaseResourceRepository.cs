using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using Kjkardum.CloudyBack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Kjkardum.CloudyBack.Infrastructure.Repositories;

public class VirtualNetworkableBaseResourceRepository(
    ApplicationDbContext dbContext): IVirtualNetworkableBaseResourceRepository
{
    public async Task<VirtualNetworkableBaseResource?> GetById(Guid id)
    {
        var virtualNetworkableBaseResource = await dbContext.VirtualNetworkableBaseResources
            .Include(t => t.PublicProxyConfigurations)
            .Include(t => t.VirtuaLNetworks)!
            .ThenInclude(t => t.VirtualNetwork)
            .Include(t => t.ResourceGroup)
            .FirstOrDefaultAsync(x => x.Id == id);

        return virtualNetworkableBaseResource;
    }

    public async Task AddProxyConfiguration(PublicProxyConfiguration publicProxyConfiguration)
    {
        await dbContext.AddAsync(publicProxyConfiguration);
        await dbContext.SaveChangesAsync();
    }

    public async Task RemoveProxyConfiguration(Guid id)
    {
        var proxyConfiguration = await dbContext.PublicProxyConfigurations.FindAsync(id);
        if (proxyConfiguration != null)
        {
            dbContext.PublicProxyConfigurations.Remove(proxyConfiguration);
            await dbContext.SaveChangesAsync();
        }
    }
}
