import { useGetApiAuthenticationAuthenticatedQuery } from '../src/services/rtk/cloudyApi';
import { useNavigate } from 'react-router-dom';
import { ReactNode, useEffect } from 'react';

export const AnonymousGuard = ({ children }: { children: ReactNode }) => {
  const navigate = useNavigate();
  const { isSuccess, error, isLoading } = useGetApiAuthenticationAuthenticatedQuery();

  useEffect(() => {
    if (!error && isSuccess && !isLoading) {
      navigate('/');
    }
  }, [error, isLoading, isLoading]);

  return <>{!isSuccess && children}</>;
}
