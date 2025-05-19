import '@mantine/core/styles.css';
import 'mantine-datatable/styles.layer.css';
import './layout.css';

import { Provider as ReduxProvider } from 'react-redux';
import { RouterProvider } from 'react-router-dom';
import { PersistGate } from 'redux-persist/integration/react';
import { MantineProvider } from '@mantine/core';
import { persistor, store } from '@/store';
import { routes } from './Router';
import { theme } from './theme';

export default function App() {
  return (
    <ReduxProvider store={store}>
      <PersistGate loading={null} persistor={persistor}>
        <MantineProvider theme={theme}>
          <RouterProvider router={routes} />
        </MantineProvider>
      </PersistGate>
    </ReduxProvider>
  );
}
