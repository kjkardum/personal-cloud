import { createBrowserRouter } from 'react-router-dom';
import Layout from '@/Layout';
import { BrowseAllResourcePage } from '@/pages/browse/browseAllResource.page';
import { ViewCloudyDockerPage } from '@/pages/cloudyDocker/view/ViewCloudyDocker.page';
import { NewKafkaPage } from '@/pages/kafka/createNew/NewKafka.page';
import { ViewKafkaClusterPage } from '@/pages/kafka/view/viewKafkaCluster.page';
import { NewPostgresDatabasePage } from '@/pages/postgres/createNew/NewPostgresDatabase.page';
import { NewPostgresServerPage } from '@/pages/postgres/createNew/NewPostgresServer.page';
import { ViewPostgresServerPage } from '@/pages/postgres/view/viewPostgresServer.page';
import { ViewResourceGroupPage } from '@/pages/resourceGroup/view/viewResourceGroup.page';
import { NewVirtualNetworkPage } from '@/pages/virtualNetwork/createNew/NewVirtualNetwork.page';
import { ViewVirtualNetworkPage } from '@/pages/virtualNetwork/view/viewVirtualNetwork.page';
import { NewWebApplicationPage } from '@/pages/webApplication/createNew/NewWebApplication.page';
import { HomePage } from './pages/Home.page';
import { ViewPostgresDatabasePage } from './pages/postgres/view/viewPostgresDatabase.page';
import { ViewWebApplicationPage } from './pages/webApplication/view/viewWebApplication.page';
import { BrowseWebApplicationResourcePage } from '@/pages/browse/browseWebApplicationResource.page';
import { BrowsePostgresDatabaseResourcePage } from '@/pages/browse/browsePostgresDatabaseResource.page';
import { LoginPage } from '@/pages/login/login.page';


export const routes = createBrowserRouter([
  {
    path: '/login',
    element: <LoginPage />
  },
  {
    path: '/',
    element: <Layout />,
    children: [
      {
        path: '/',
        element: <HomePage />,
      },
      {
        path: '/browse',
        children: [
          {
            path: 'all',
            element: <BrowseAllResourcePage resourceType={undefined} />,
          },
          {
            path: 'rg',
            element: <BrowseAllResourcePage resourceType="ResourceGroup" />,
          },
          {
            path: 'vnet',
            element: <BrowseAllResourcePage resourceType="VirtualNetworkResource" />,
          },
          {
            path: 'kafka',
            element: <BrowseAllResourcePage resourceType="KafkaClusterResource" />,
          },
          {
            path: 'webapp',
            element: <BrowseWebApplicationResourcePage />,
          },
          {
            path: 'psqldb',
            element: <BrowsePostgresDatabaseResourcePage />,
          }
        ],
      },
      {
        path: 'cloudyDocker',
        children: [
          {
            path: 'view',
            element: <ViewCloudyDockerPage />,
          },
        ],
      },
      {
        path: 'postgres',
        children: [
          {
            path: 'new',
            children: [
              {
                path: 'database',
                element: <NewPostgresDatabasePage />,
              },
              {
                path: 'server',
                element: <NewPostgresServerPage />,
              },
            ],
          },
          {
            path: 'view',
            children: [
              {
                path: 'server/:id',
                element: <ViewPostgresServerPage />,
              },
              {
                path: 'database/:id',
                element: <ViewPostgresDatabasePage />,
              },
            ],
          },
        ],
      },
      {
        path: 'kafka',
        children: [
          {
            path: 'new',
            children: [
              {
                path: 'cluster',
                element: <NewKafkaPage />,
              },
            ],
          },
          {
            path: 'view',
            children: [
              {
                path: 'cluster/:id',
                element: <ViewKafkaClusterPage />,
              },
            ],
          },
        ],
      },
      {
        path: 'webApplication',
        children: [
          {
            path: 'new',
            element: <NewWebApplicationPage />,
          },
          {
            path: 'view/:id',
            element: <ViewWebApplicationPage />,
          },
        ],
      },
      {
        path: 'virtualNetwork',
        children: [
          {
            path: 'new',
            element: <NewVirtualNetworkPage />,
          },
          {
            path: 'view/:id',
            element: <ViewVirtualNetworkPage />,
          },
        ],
      },
      {
        path: 'resourceGroup',
        children: [
          {
            path: 'view/:id',
            element: <ViewResourceGroupPage />,
          },
        ],
      },
    ],
  },
]);
