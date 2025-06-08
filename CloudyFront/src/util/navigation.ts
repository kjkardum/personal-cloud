export const viewResourceOfType = (resourceType: string, resourceId: string | undefined) => {
  switch (resourceType) {
    case 'PostgresServerResource':
      return `/postgres/view/server/${resourceId}`;
    case 'PostgresDatabaseResource':
      return `/postgres/view/database/${resourceId}`;
    case 'KafkaClusterResource':
      return `/kafka/view/cluster/${resourceId}`;
    case 'WebApplicationResource':
      return `/webApplication/view/${resourceId}`;
    case 'ResourceGroup':
      return `/resourceGroup/view/${resourceId}`;
    case 'VirtualNetworkResource':
      return `/virtualNetwork/view/${resourceId}`;
    default:
      console.error(`Unknown resource type: ${resourceType}`);
      return '/'; // Fallback to home or some default route
  }
}

export const viewVirtualResource = (resourceType: string) => {
  switch (resourceType) {
    case 'CloudyDocker':
      return '/cloudyDocker/view';
    default:
      console.error(`Unknown virtual resource type: ${resourceType}`);
      return '/'; // Fallback to home or some default route
  }
}
