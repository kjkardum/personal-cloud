import { z } from 'zod';
import { Button, Divider, TextInput } from '@mantine/core';
import { useForm, zodResolver } from '@mantine/form';
import { ResourceGroupAutocomplete } from '@/components/ResourceGroup/ResourceGroupAutocomplete';
import {
  CreatePostgresServerCommand,
  usePostApiResourcePostgresServerResourceMutation,

} from '@/services/rtk/cloudyApi';
import {useNavigate} from "react-router-dom";

export const NewPostgresServerForm = () => {
  const [createPostgresServerResource, { isLoading }] = usePostApiResourcePostgresServerResourceMutation();
    const navigate = useNavigate();
  const form = useForm<CreatePostgresServerCommand>({
    mode: 'uncontrolled',
    initialValues: { resourceGroupId: '', serverName: '', serverPort: undefined },
    validate: zodResolver(
      z.object({
        resourceGroupId: z
          .string()
          .nonempty('Resource group is required, select or type any name to create a new one'),
        serverName: z.string().nonempty('Server name is required'),
        serverPort: z.number().optional(),
      })
    ),
  });

  const submitForm = async (values: CreatePostgresServerCommand) => {
    const server = await createPostgresServerResource({ createPostgresServerCommand: values }).unwrap();
    navigate(`/postgres/new/database?serverId=${server.id}`);
  };
  return (
    <form onSubmit={form.onSubmit(submitForm)}>
      <ResourceGroupAutocomplete
        onResourceGroupSelect={(resourceGroupId) =>
          form.setFieldValue('resourceGroupId', resourceGroupId)
        }
      />
      <TextInput
        label="Server name"
        placeholder="Server name"
        key={form.key('serverName')}
        {...form.getInputProps('serverName')}
      />
      <Divider my="sm" />
      <Button type="submit" loading={isLoading}>Create database server</Button>
    </form>
  );
};
