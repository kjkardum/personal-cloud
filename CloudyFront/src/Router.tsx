import { createBrowserRouter } from 'react-router-dom';
import { HomePage } from './pages/Home.page';
import Layout from "@/Layout";
import {NewPostgresServerPage} from "@/pages/database/createNew/postgresServer/NewPostgresServer.page";
import {NewPostgresDatabasePage} from "@/pages/database/createNew/postgresDb/NewPostgresDatabase.page";

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
        path: 'postgres/new',
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
      }
    ]
  }
]);
