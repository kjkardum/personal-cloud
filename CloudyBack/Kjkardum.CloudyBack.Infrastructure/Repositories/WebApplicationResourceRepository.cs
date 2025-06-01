using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using Kjkardum.CloudyBack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Kjkardum.CloudyBack.Infrastructure.Repositories;

public class WebApplicationResourceRepository(ApplicationDbContext dbContext): IWebApplicationResourceRepository
{
    public async Task<WebApplicationResource> Create(WebApplicationResource kafkaClusterResource)
    {
        await dbContext.WebApplicationResources.AddAsync(kafkaClusterResource);
        await dbContext.SaveChangesAsync();
        return kafkaClusterResource;
    }

    public async Task<WebApplicationResource?> GetById(Guid id)
    {
        var webApplicationResource = await dbContext.WebApplicationResources
            .Include(t => t.VirtuaLNetworks)!
            .ThenInclude(t => t.VirtualNetwork)
            .Include(t => t.ResourceGroup)
            .Include(t => t.Configuration)
            .FirstOrDefaultAsync(x => x.Id == id);

        return webApplicationResource;
    }

    public async Task<WebApplicationResource?> Update(WebApplicationResource resource)
    {
        dbContext.WebApplicationResources.Update(resource);
        await dbContext.SaveChangesAsync();
        return resource;
    }

    public async Task Delete(WebApplicationResource resource)
    {
        dbContext.WebApplicationResources.Remove(resource);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpsertConfigurationEntry(WebApplicationConfigurationEntry configurationEntry)
    {
        configurationEntry.Key = configurationEntry.Key.Trim();
        var existingEntry = await dbContext.WebApplicationConfigurationEntries
            .FirstOrDefaultAsync(x => x.Key == configurationEntry.Key &&
                x.WebApplicationResourceId == configurationEntry.WebApplicationResourceId);

        if (existingEntry != null)
        {
            existingEntry.Value = configurationEntry.Value;
            dbContext.WebApplicationConfigurationEntries.Update(existingEntry);
        }
        else
        {
            await dbContext.WebApplicationConfigurationEntries.AddAsync(configurationEntry);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteConfigurationEntry(WebApplicationConfigurationEntry configurationEntry)
    {
        var existingEntries = await dbContext.WebApplicationConfigurationEntries
            .Where(x => x.Key == configurationEntry.Key &&
                x.WebApplicationResourceId == configurationEntry.WebApplicationResourceId)
            .ToListAsync();

        if (existingEntries.Count == 0)
        {
            return;
        }

        dbContext.WebApplicationConfigurationEntries.RemoveRange(existingEntries);
        await dbContext.SaveChangesAsync();
    }
}
