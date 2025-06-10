import { useGetApiAuthenticationAuthenticatedQuery } from '../src/services/rtk/cloudyApi';
import { ReactNode, useEffect } from 'react';
import { Box } from '@mantine/core';
import { LoginPage } from '../src/pages/login/login.page';

export const AuthGuard = ({ children }: {
  children: ReactNode;
}) => {
  const { isLoading, isError, isSuccess } = useGetApiAuthenticationAuthenticatedQuery();
  useEffect(() => {
  }, [isLoading, isError, isSuccess]);

  return (
    <>
      {isLoading && !isError && (
        <Box w="100vw" h="100vh" display="flex" style={{justifyContent: 'center', alignItems: 'center'}}>
          <Box>Loading...</Box>
        </Box>
      )}
      {((!isLoading && !isSuccess) || isError) && <LoginPage />}
      {!isLoading && isSuccess && children}
    </>
  );
}
