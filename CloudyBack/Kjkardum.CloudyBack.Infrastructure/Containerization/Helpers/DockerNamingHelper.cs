namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Helpers;

public class DockerNamingHelper
{
    public static string GetVolumeName(Guid id) => $"cloudy{id:N}volume";
    public static string GetScalableVolumeName(Guid id, int index) => $"cloudy{id:N}volumescale{index}";
    public static string GetContainerName(Guid id) => $"cloudy{id:N}container";
    public static string GetScalableContainerName(Guid id, int index) => $"cloudy{id:N}containerscale{index}";
    public static string GetSidecarName(Guid id, string type) => $"cloudy{id:N}sidecar{type}";
    public static string GetSidecarTelemetryName(Guid id) => GetSidecarName(id, "telemetry");
    public static string GetNetworkName(Guid id) => $"cloudy{id:N}network";
    public static string GetVirtualNetworkResourceName(Guid id) => $"cloudy{id:N}virtualnetwork";
    public const string ObservabilityNetworkName  = "cloudyobservabilitynetwork";
    public const string PrometheusVolumeName = "cloudyprometheusvolume";
    public const string PrometheusContainerName = "cloudyprometheuscontainer";
    public const string LokiVolumeName = "cloudylokivolume";
    public const string LokiContainerName = "cloudylokicontainer";
    public const string CadvisorContainerName = "cloudycadvisorcontainer";
    public const string WebApplicationBuilderImageName = "cloudywebapplicationbuilder";
    public const string CaddyBuilderImageName = "cloudycaddybuilder";
    public const string CaddyVolumeName = "cloudycaddyvolume";
    public const string CaddyContainerName = "cloudycaddycontainer";
    public const string LokiNetworkName = "cloudylokinetwork";
    public const string PrometheusNetworkName = "cloudyprometheusnetwork";
    public const string GrafanaContainerName = "cloudygrafanacontainer";
    public const string GrafanaVolumeName = "cloudygrafanavolume";
    public const string PostgresBuilderImageName = "cloudypostgresbuilder";
}
