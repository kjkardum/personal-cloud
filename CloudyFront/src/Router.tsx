import { createBrowserRouter } from 'react-router-dom';
import { HomePage } from './pages/Home.page';
import Layout from "@/Layout";
import {NewPostgresServerPage} from "@/pages/postgres/createNew/NewPostgresServer.page";
import {NewPostgresDatabasePage} from "@/pages/postgres/createNew/NewPostgresDatabase.page";
import { ViewPostgresServerPage } from '@/pages/postgres/view/viewPostgresServer.page';

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
              }
            ]
          }
        ]
      }
    ]
  }
]);
