using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using Kjkardum.CloudyBack.Infrastructure.Persistence;

namespace Kjkardum.CloudyBack.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    public async Task<User> Create(User user)
    {
        await dbContext.AddAsync(user);
        await dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<User> UpdateLastLogin(User user)
    {
        user.LastLogin = DateTime.UtcNow;
        dbContext.Update(user);
        await dbContext.SaveChangesAsync();
        return user;
    }
    public Task Update(User user)
    {
        dbContext.Update(user);
        return dbContext.SaveChangesAsync();
    }

    public Task Delete(User user)
    {
        dbContext.Remove(user);
        return dbContext.SaveChangesAsync();
    }

    public async Task<User?> GetByEmail(string email)
        => await dbContext.Users.SingleOrDefaultAsync(t => t.Email == email);

    public Task<User?> GetByUserId(Guid userId)
        => dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);

    public Task<bool> DoesUserExist(string email)
        => dbContext.Users.AnyAsync(t => t.Email == email);
}
