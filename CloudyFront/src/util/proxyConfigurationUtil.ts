export const CanTypeUseHttp = (resourceType: string) => {
  switch (resourceType) {
    case 'PostgresServerResource':
      return false;
    case 'KafkaClusterResource':
      return false;
    case 'WebApplicationResource':
      return true;
    default:
      return false;
  }
}
