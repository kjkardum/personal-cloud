import '@mantine/core/styles.css';

import { Outlet, RouterProvider } from 'react-router-dom';
import { MantineProvider } from '@mantine/core';
import Layout from '@/Layout';
import { routes } from './Router';
import { theme } from './theme';

export default function App() {
  return (
    <MantineProvider theme={theme}>
      <RouterProvider router={routes}/>
    </MantineProvider>
  );
}
