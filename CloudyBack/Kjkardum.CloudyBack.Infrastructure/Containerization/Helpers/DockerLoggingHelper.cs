using Docker.DotNet.Models;

namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Helpers;

public static class DockerLoggingHelper
{
    public static LogConfig DefaultLogConfig(Guid id) => new()
    {
        Type = "loki",
        Config = new Dictionary<string, string>
        {
            { "loki-url", "http://localhost:13100/loki/api/v1/push" },
            { "loki-retries", "5" },
            { "loki-batch-size", "400" },
            { "env", "CLOUDY_RESOURCE_ID" }
        }
    };

    public static string LogEnvironmentVariableValue(Guid id) => $"{id:N}";
    public static string LogEnvironmentVariable(Guid id) => $"CLOUDY_RESOURCE_ID={LogEnvironmentVariableValue(id)}";
}
