using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Repositories;

public interface IUserRepository
{
    Task<User> Create(User user);
    Task<User?> GetByEmail(string email);
    Task<User?> GetByUserId(Guid userId);
    Task<bool> DoesUserExist(string email);
    Task<User> UpdateLastLogin(User user);
    Task Update(User user);
    Task Delete(User user);
}
