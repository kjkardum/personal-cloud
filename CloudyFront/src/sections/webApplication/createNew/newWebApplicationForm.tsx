import { useNavigate } from 'react-router-dom';
import { z } from 'zod';
import { Button, Divider, Select, TextInput } from '@mantine/core';
import { useForm, zodResolver } from '@mantine/form';
import { ResourceGroupAutocomplete } from '@/components/ResourceGroup/ResourceGroupAutocomplete';
import {
  CreateWebApplicationResourceCommand,
  usePostApiResourceWebApplicationResourceMutation,
  WebApplicationSourceType,
} from '@/services/rtk/cloudyApi';
import { viewResourceOfType } from '@/util/navigation';

type sourceTypeEnum = { [enumValue in WebApplicationSourceType]: string };
const sourceTypeEnumOptions: sourceTypeEnum = {
  [WebApplicationSourceType.PublicGitClone]: 'Public Git Clone',
};

const sourceTypeEnumValueAsList = Object.entries(sourceTypeEnumOptions).map(([value, label]) => ({
  value: value as WebApplicationSourceType,
  label,
}));

export const NewWebApplicationForm = () => {
  const [createWebApplicationResource] = usePostApiResourceWebApplicationResourceMutation();
  const navigate = useNavigate();
  const form = useForm<CreateWebApplicationResourceCommand>({
    mode: 'uncontrolled',
    initialValues: {
      resourceGroupId: '',
      webApplicationName: '',
      sourcePath: '',
      sourceType: WebApplicationSourceType.PublicGitClone,
    },
    validate: zodResolver(
      z.object({
        resourceGroupId: z
          .string()
          .nonempty('Resource group is required, select or type any name to create a new one'),
        webApplicationName: z.string().nonempty('Web application name is required'),
        sourcePath: z.string().nonempty('Deployment URL is required'),
        sourceType: z.nativeEnum(WebApplicationSourceType, {
          message: 'Deployment method is required',
        }),
      })
    ),
  });

  const submitForm = async (values: CreateWebApplicationResourceCommand) => {
    const server = await createWebApplicationResource({
      createWebApplicationResourceCommand: values,
    }).unwrap();
    navigate(viewResourceOfType('WebApplicationResource', server.id));
  };
  return (
    <form onSubmit={form.onSubmit(submitForm)}>
      <ResourceGroupAutocomplete
        onResourceGroupSelect={(resourceGroupId) =>
          form.setFieldValue('resourceGroupId', resourceGroupId)
        }
      />
      <TextInput
        label="Web application name"
        placeholder="Web application name"
        key={form.key('webApplicationName')}
        {...form.getInputProps('webApplicationName')}
      />
      <Select
        label="Deployment method"
        placeholder="Deployment method"
        data={sourceTypeEnumValueAsList}
        allowDeselect={false}
        key={form.key('sourceType')}
        {...form.getInputProps('sourceType')}
      />
      <TextInput
        label="Deployment url"
        placeholder="Deployment url"
        key={form.key('sourcePath')}
        {...form.getInputProps('sourcePath')}
      />
      <Divider my="sm" />
      <Button type="submit">Create web application</Button>
    </form>
  );
};
