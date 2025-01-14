import { createBrowserRouter } from 'react-router-dom';
import { HomePage } from './pages/Home.page';
import {NewPostgres} from "@/pages/database/createNew/postgres/NewPostgres.page";
import Layout from "@/Layout";

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
        path: 'database/createNew',
        children: [
          {
            path: 'postgres',
            element: <NewPostgres/>,
          }
        ]
      }
    ]
  }
]);
