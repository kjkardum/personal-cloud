namespace Kjkardum.CloudyBack.Application.Configuration;

public class AppConfiguration
{
    public const string ConfigKey = "ApplicationConfiguration";
    public string ApplicationName { get; set; } = null!;
    public string FrontendUrl { get; set; } = null!;
}
