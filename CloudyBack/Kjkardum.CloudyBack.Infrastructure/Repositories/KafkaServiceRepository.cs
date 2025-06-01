using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using Kjkardum.CloudyBack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Kjkardum.CloudyBack.Infrastructure.Repositories;

public class KafkaServiceRepository(ApplicationDbContext dbContext): IKafkaServiceRepository
{
    public async Task<KafkaClusterResource> Create(KafkaClusterResource kafkaClusterResource)
    {
        await dbContext.KafkaClusterResources.AddAsync(kafkaClusterResource);
        await dbContext.SaveChangesAsync();
        return kafkaClusterResource;
    }

    public async Task<KafkaClusterResource?> GetById(Guid id)
    {
        var kafkaClusterResource = await dbContext.KafkaClusterResources
            .Include(t => t.VirtuaLNetworks)!
            .ThenInclude(t => t.VirtualNetwork)
            .Include(t => t.ResourceGroup)
            .FirstOrDefaultAsync(x => x.Id == id);

        return kafkaClusterResource;
    }

    public async Task Delete(KafkaClusterResource resource)
    {
        dbContext.KafkaClusterResources.Remove(resource);
        await dbContext.SaveChangesAsync();
    }
}
