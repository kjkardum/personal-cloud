namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Helpers;

public class DockerNamingHelper
{
    public static string GetVolumeName(Guid id) => $"cloudy{id:N}volume";
    public static string GetContainerName(Guid id) => $"cloudy{id:N}container";
    public static string GetSidecarName(Guid id, string type) => $"cloudy{id:N}sidecar{type}";
    public static string GetSidecarTelemetryName(Guid id) => GetSidecarName(id, "telemetry");
    public static string GetNetworkName(Guid id) => $"cloudy{id:N}network";
    public const string ObservabilityNetworkName  = "cloudyobservabilitynetwork";
    public const string PrometheusVolumeName = "cloudyprometheusvolume";
    public const string PrometheusContainerName = "cloudyprometheuscontainer";
    public const string LokiVolumeName = "cloudylokivolume";
    public const string LokiContainerName = "cloudylokicontainer";
    public const string CadvisorContainerName = "cloudycadvisorcontainer";
}
