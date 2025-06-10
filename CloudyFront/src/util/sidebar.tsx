import {
  CloudyIconDatabase,
  CloudyIconWebApplication,
  CloudyIconBrowseResources,
  CloudyIconResourceGroup,
  CloudyIconHomepage, CloudyIconVirtualNetworkResource, CloudyIconKafkaResource, CloudyIconDocker,
} from '@/icons/Resources';

export const sidebarItems = [
    {
        name: "Homepage",
        icon: <CloudyIconHomepage />,
        href: '/'
    },
    {
        name: "Browse resources",
        icon: <CloudyIconBrowseResources />,
        href: '/browse/all'
    },
    {
      name: "Host resource",
      icon: <CloudyIconDocker />,
      href: '/cloudyDocker/view'
    },
    {
        name: "Web applications",
        icon: <CloudyIconWebApplication />,
        href: '/browse/webapp'
    },
    {
        name: "Databases",
        icon: <CloudyIconDatabase />,
        href: '/browse/psqldb'
    },
    {
        name: "Resource groups",
        icon: <CloudyIconResourceGroup />,
        href: '/browse/rg'
    },
    {
      name: "Virtual networks",
      icon: <CloudyIconVirtualNetworkResource />,
      href: '/browse/vnet'
    },
    {
      name: "Kafka clusters",
      icon: <CloudyIconKafkaResource />,
      href: '/browse/kafka'
    }
]
