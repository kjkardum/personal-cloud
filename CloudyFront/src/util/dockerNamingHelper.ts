const guidstring = (id: string) => id.replace(/-/g, ''); // Remove dashes from GUID

export const DockerNamingHelper = {
  getVolumeName: (id: string) => `cloudy${guidstring(id)}volume`,
  getContainerName: (id: string) => `cloudy${guidstring(id)}container`,
  getSidecarName: (id: string, type: string) => `cloudy${guidstring(id)}sidecar${type}`,
  getSidecarTelemetryName: (id: string) => DockerNamingHelper.getSidecarName(id, 'telemetry'),
  getNetworkName: (id: string) => `cloudy${guidstring(id)}network`,
  observabilityNetworkName: 'cloudyobservabilitynetwork',
  prometheusVolumeName: 'cloudyprometheusvolume',
  prometheusContainerName: 'cloudyprometheuscontainer',
  lokiVolumeName: 'cloudylokivolume',
  lokiContainerName: 'cloudylokicontainer',
  cadvisorContainerName: 'cloudycadvisorcontainer',
}
