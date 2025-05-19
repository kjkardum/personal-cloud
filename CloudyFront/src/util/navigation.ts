export const viewResourceOfType = (resourceType: string, resourceId: string) => {
  switch (resourceType) {
    case 'PostgresServerResource':
      return `/postgres/view/server/${resourceId}`;
    case 'PostgresDatabaseResource':
      return `/postgres/view/database/${resourceId}`;
    case 'ResourceGroup':
      return `/resourcesGroup/view/${resourceId}`;
    default:
      console.error(`Unknown resource type: ${resourceType}`);
      return '/'; // Fallback to home or some default route
  }
}
