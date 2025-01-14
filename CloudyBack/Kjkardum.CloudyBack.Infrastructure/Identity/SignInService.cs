using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Kjkardum.CloudyBack.Application.Configuration;
using Kjkardum.CloudyBack.Application.Services;
using Kjkardum.CloudyBack.Domain.Entities;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Kjkardum.CloudyBack.Infrastructure.Identity;

public class SignInService : ISignInService
{
    private readonly JwtConfiguration _jwtConfiguration;

    public SignInService(IOptions<JwtConfiguration> jwtConfiguration)
    {
        _jwtConfiguration = jwtConfiguration.Value;
    }

    public string GenerateJwToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("uid", user.Id.ToString()),
        };

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        return new JwtSecurityTokenHandler().WriteToken(
            new JwtSecurityToken(
                _jwtConfiguration.Issuer,
                _jwtConfiguration.Audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtConfiguration.DurationInMinutes),
                signingCredentials: signingCredentials));
    }

    public string? HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    public bool CheckPasswordHash(string passwordHash, string password) =>
        BCrypt.Net.BCrypt.Verify(password, passwordHash);
}
