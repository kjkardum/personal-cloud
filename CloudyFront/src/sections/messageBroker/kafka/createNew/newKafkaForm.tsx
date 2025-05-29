import { z } from 'zod';
import { Button, Divider, TextInput } from '@mantine/core';
import { useForm, zodResolver } from '@mantine/form';
import { ResourceGroupAutocomplete } from '@/components/ResourceGroup/ResourceGroupAutocomplete';
import {
  CreateKafkaClusterCommand,
  usePostApiResourceKafkaClusterResourceMutation,
} from '@/services/rtk/cloudyApi';
import {useNavigate} from "react-router-dom";
import { viewResourceOfType } from '@/util/navigation';

export const NewKafkaForm = () => {
  const [createKafkaResource] = usePostApiResourceKafkaClusterResourceMutation();
  const navigate = useNavigate();
  const form = useForm<CreateKafkaClusterCommand>({
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

  const submitForm = async (values: CreateKafkaClusterCommand) => {
    const server = await createKafkaResource({ createKafkaClusterCommand: values }).unwrap();
    navigate(viewResourceOfType('KafkaClusterResource', server.id));
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
      <TextInput
        type="number"
        label="Server port"
        placeholder="Server port (leave empty for default)"
        key={form.key('serverPort')}
        {...form.getInputProps('serverPort')}
      />
      <Divider my="sm" />
      <Button type="submit">Create kafka cluster</Button>
    </form>
  );
};
