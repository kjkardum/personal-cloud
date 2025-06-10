import { Box, Button, Divider, TextInput } from '@mantine/core';
import { useForm } from '@mantine/form';
import {
  useGetApiAuthenticationAuthenticatedQuery,
  usePostApiAuthenticationLoginMutation,
  UserLoginCommand,
} from '@/services/rtk/cloudyApi';
import { AnonymousGuard } from '../../../guards/AnonymousGuard';

export const LoginPage = () => {
  const [authenticate, { isLoading }] = usePostApiAuthenticationLoginMutation();
  const { refetch } = useGetApiAuthenticationAuthenticatedQuery();
  const form = useForm<UserLoginCommand>({
    mode: 'uncontrolled',
    initialValues: { email: '', password: '' },
  });

  const submitForm = async (values: UserLoginCommand) => {
    await authenticate({ userLoginCommand: values }).unwrap();
    form.reset();
    refetch();
  };
  return (
    <AnonymousGuard>
      <form onSubmit={form.onSubmit(submitForm)}>
        <Box
          w="100vw"
          h="100vh"
          display="flex"
          style={{ justifyContent: 'center', alignItems: 'center' }}
        >
          <Box w={400} p="md" style={{ backgroundColor: 'rgba(128,128,128,0.3)', borderRadius: 8 }}>
            <TextInput
              label="Email"
              placeholder="Email"
              key={form.key('email')}
              {...form.getInputProps('email')}
            />
            <TextInput
              label="Password"
              placeholder="Password"
              type="password"
              key={form.key('password')}
              {...form.getInputProps('password')}
            />
            <Divider my="sm" />
            <Button type="submit" loading={isLoading}>
              Login
            </Button>
          </Box>
        </Box>
      </form>
    </AnonymousGuard>
  );
};
