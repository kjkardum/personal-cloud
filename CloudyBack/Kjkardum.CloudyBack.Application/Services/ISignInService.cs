using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Services;

public interface ISignInService
{
    public string GenerateJwToken(User user);
    public string? HashPassword(string password);
    public bool CheckPasswordHash(string passwordHash, string password);
}
