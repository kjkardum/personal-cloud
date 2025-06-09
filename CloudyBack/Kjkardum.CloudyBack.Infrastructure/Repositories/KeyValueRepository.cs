using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Kjkardum.CloudyBack.Infrastructure.Repositories;

public class KeyValueRepository(ApplicationDbContext dbContext): IKeyValueRepository
{
    public async Task<string?> GetValueAsync(string key)
    {
        var kv = await dbContext.KeyValueTableEntries.FirstOrDefaultAsync(kv => kv.Key == key);
        return kv?.Value;
    }

    public async Task SetValueAsync(string key, string value)
    {
        var kv = await dbContext.KeyValueTableEntries.FirstOrDefaultAsync(kv => kv.Key == key);
        if (kv == null)
        {
            kv = new Domain.Entities.KeyValueTableEntry { Key = key, Value = value };
            await dbContext.KeyValueTableEntries.AddAsync(kv);
        }
        else
        {
            kv.Value = value;
            dbContext.KeyValueTableEntries.Update(kv);
        }
        await dbContext.SaveChangesAsync();
    }
}
