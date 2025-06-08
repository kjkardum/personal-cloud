import { createBrowserRouter } from 'react-router-dom';
import Layout from '@/Layout';
import { NewKafkaPage } from '@/pages/kafka/createNew/NewKafka.page';
import { ViewKafkaClusterPage } from '@/pages/kafka/view/viewKafkaCluster.page';
import { NewPostgresDatabasePage } from '@/pages/postgres/createNew/NewPostgresDatabase.page';
import { NewPostgresServerPage } from '@/pages/postgres/createNew/NewPostgresServer.page';
import { ViewPostgresServerPage } from '@/pages/postgres/view/viewPostgresServer.page';
import { ViewResourceGroupPage } from '@/pages/resourceGroup/view/viewResourceGroup.page';
import { NewWebApplicationPage } from '@/pages/webApplication/createNew/NewWebApplication.page';
import { HomePage } from './pages/Home.page';
import { ViewPostgresDatabasePage } from './pages/postgres/view/viewPostgresDatabase.page';
import { ViewWebApplicationPage } from './pages/webApplication/view/viewWebApplication.page';
import { NewVirtualNetworkPage } from '@/pages/virtualNetwork/createNew/NewVirtualNetwork.page';
import { ViewVirtualNetworkPage } from '@/pages/virtualNetwork/view/viewVirtualNetwork.page';
import { ViewCloudyDockerPage } from '@/pages/cloudyDocker/view/ViewCloudyDocker.page';


export const routes = createBrowserRouter([
  {
    path: '/',
    element: <Layout />,
    children: [
      {
        path: '/',
        element: <HomePage />,
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
