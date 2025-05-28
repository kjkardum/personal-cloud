import { createBrowserRouter } from 'react-router-dom';
import Layout from "@/Layout";
import { NewPostgresDatabasePage } from "@/pages/postgres/createNew/NewPostgresDatabase.page";
import { NewPostgresServerPage } from "@/pages/postgres/createNew/NewPostgresServer.page";
import { ViewPostgresServerPage } from '@/pages/postgres/view/viewPostgresServer.page';
import { HomePage } from './pages/Home.page';
import { ViewPostgresDatabasePage } from './pages/postgres/view/viewPostgresDatabase.page';
import { ViewResourceGroupPage } from '@/pages/resourceGroup/view/viewResourceGroup.page';


export const routes = createBrowserRouter([
  {
    path: '/',
    element: <Layout/>,
    children: [
      {
        path: '/',
        element: <HomePage/>,
      },
      {
        path: 'postgres',
        children: [
          {
            path: 'new',
            children: [
              {
                path: 'database',
                element: <NewPostgresDatabasePage/>,
              },
              {
                path: 'server',
                element: <NewPostgresServerPage/>,
              }
            ]
          },
          {
            path: 'view',
            children: [
              {
                path: 'server/:id',
                element: <ViewPostgresServerPage/>,
              },
              {
                path: 'database/:id',
                element: <ViewPostgresDatabasePage/>,
              }
            ]
          }
        ]
      },
      {
        path: 'resourceGroup',
        children: [
          {
            path: 'view/:id',
            element: <ViewResourceGroupPage />,
          }
        ]
      }
    ]
  }
]);
