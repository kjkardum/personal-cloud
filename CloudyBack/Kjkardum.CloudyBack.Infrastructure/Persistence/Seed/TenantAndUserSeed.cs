using Kjkardum.CloudyBack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kjkardum.CloudyBack.Infrastructure.Persistence.Seed;

public static class TenantAndUserSeed
{
    public static async Task Seed(ApplicationDbContext context)
    {
        if (!await context.Users.AnyAsync())
        {
            var users = new List<User>
            {
                new()
                {
                    Id = Guid.Empty,
                    Email = "admin@cloudy.diy",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pa$$w0rdDiy"),
                    SuperAdmin = true
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }

        _ = await context.ResourceGroupedBaseResources.Take(5).ToListAsync();
    }
}
