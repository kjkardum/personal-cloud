import {
  CreateKafkaClusterCommand, CreateVirtualNetworkResourceCommand,
  usePostApiResourceKafkaClusterResourceMutation,
  usePostApiResourceVirtualNetworkResourceMutation,
} from '@/services/rtk/cloudyApi';
import { useNavigate } from 'react-router-dom';
import { useForm, zodResolver } from '@mantine/form';
import { z } from 'zod';
import { viewResourceOfType } from '@/util/navigation';
import { ResourceGroupAutocomplete } from '@/components/ResourceGroup/ResourceGroupAutocomplete';
import { Button, Divider, TextInput } from '@mantine/core';

export const NewVirtualNetworkForm = () => {
  const [createVirtualNetworkResource] = usePostApiResourceVirtualNetworkResourceMutation();
  const navigate = useNavigate();
  const form = useForm<CreateVirtualNetworkResourceCommand>({
    mode: 'uncontrolled',
    initialValues: { resourceGroupId: '', name: '' },
    validate: zodResolver(
      z.object({
        resourceGroupId: z
          .string()
          .nonempty('Resource group is required, select or type any name to create a new one'),
        name: z.string().nonempty('Server name is required')
      })
    ),
  });

  const submitForm = async (values: CreateVirtualNetworkResourceCommand) => {
    const server = await createVirtualNetworkResource({ createVirtualNetworkResourceCommand: values }).unwrap();
    navigate(viewResourceOfType('VirtualNetworkResource', server.id));
  };
  return (
    <form onSubmit={form.onSubmit(submitForm)}>
      <ResourceGroupAutocomplete
        onResourceGroupSelect={(resourceGroupId) =>
          form.setFieldValue('resourceGroupId', resourceGroupId)
        }
      />
      <TextInput
        label="Virtual network name"
        placeholder="Virtual network name"
        key={form.key('name')}
        {...form.getInputProps('name')}
      />
      <Divider my="sm" />
      <Button type="submit">Create virtual network</Button>
    </form>
  );
}
