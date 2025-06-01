import {
  CloudyIconDatabase,
  CloudyIconDatabaseServer,
  CloudyIconKafkaResource,
  CloudyIconResourceGroup, CloudyIconVirtualNetworkResource, CloudyIconWebApplication,
} from '@/icons/Resources';
import {ReactElement} from "react";

export const TypeToIcon: {[key: string]: ReactElement} = {
    'PostgresServerResource': <CloudyIconDatabaseServer />,
    'PostgresDatabaseResource': <CloudyIconDatabase />,
    'ResourceGroup': <CloudyIconResourceGroup />,
    'KafkaClusterResource': <CloudyIconKafkaResource />,
    'VirtualNetworkResource': <CloudyIconVirtualNetworkResource />,
    'WebApplicationResource': <CloudyIconWebApplication />,
}

export const TypeToText: {[key: string]: string} = {
    'PostgresServerResource': 'Postgres Server',
    'PostgresDatabaseResource': 'Postgres Database',
    'ResourceGroup': 'Resource Group',
    'KafkaClusterResource': 'Kafka Cluster',
    'VirtualNetworkResource': 'Virtual Network',
    'WebApplicationResource': 'Web Application',
}
