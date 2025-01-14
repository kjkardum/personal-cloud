namespace Kjkardum.CloudyBack.Application.Configuration;

public class JwtConfiguration
{
    public const string ConfigKey = "Jwt";

    public string Key { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public double DurationInMinutes { get; set; }
}
