import { useCallback } from 'react';
import { IconCloudUp } from '@tabler/icons-react';
import { z } from 'zod';
import { NumberInput, Select, Stack, TextInput, useMantineTheme } from '@mantine/core';
import { useForm, zodResolver } from '@mantine/form';
import { ResourceViewToolbar, ResourceViewToolbarItem } from '@/components/ResourceView/ResourceViewToolbar';
import { UpdateWebApplicationDeploymentConfigurationCommand, usePutApiResourceWebApplicationResourceByIdDeploymentConfigurationMutation, WebApplicationResourceDto, WebApplicationRuntimeType } from '@/services/rtk/cloudyApi';


type runtimeTypeEnum = { [enumValue in WebApplicationRuntimeType]: string };
const runtimeTypeEnumOptions: runtimeTypeEnum = {
  [WebApplicationRuntimeType.DotNet]: 'ASP.NET 8',
  [WebApplicationRuntimeType.NodeJs]: 'Node.js 20',
  [WebApplicationRuntimeType.Python]: 'Python 3.11',
};

const runtimeTypeEnumValueAsList = Object.entries(runtimeTypeEnumOptions).map(([value, label]) => ({
  value: value as WebApplicationRuntimeType,
  label,
}));

export const WebApplicationDeploymentSubpage = ({
  resourceBaseData,
}: {
  resourceBaseData: WebApplicationResourceDto;
}) => {
  const theme = useMantineTheme();
  const [updateWebApplicationDeployment, {isLoading: updatingWebApplication}] =
    usePutApiResourceWebApplicationResourceByIdDeploymentConfigurationMutation();
  const form = useForm<UpdateWebApplicationDeploymentConfigurationCommand>({
    mode: 'uncontrolled',
    initialValues: {
      buildCommand: resourceBaseData.buildCommand,
      startupCommand: resourceBaseData.startupCommand,
      runtimeType: resourceBaseData.runtimeType,
      port: resourceBaseData.port,
    },
    validate: zodResolver(
      z.object({
        buildCommand: z.string().optional(),
        startupCommand: z.string().nonempty('Startup command is required'),
        port: z.number().refine((value) => value > 2000 && value < 65536, {
          message: 'Port must be between 2000 and 65536, or left empty for default',
        }),
      })
    ),
  });
  const handleSubmit = useCallback(
    async (values: UpdateWebApplicationDeploymentConfigurationCommand) => {
      const server = await updateWebApplicationDeployment({
        updateWebApplicationDeploymentConfigurationCommand: values,
        id: resourceBaseData.id,
      }).unwrap();
      form.reset();
    },
    [updateWebApplicationDeployment, resourceBaseData.id, form]
  );
  const submitForm = form.onSubmit(handleSubmit);
  return (
    <form onSubmit={form.onSubmit(handleSubmit)}>
        <ResourceViewToolbar>
          <ResourceViewToolbarItem
            label="Save and deploy"
            disabled={form.submitting}
            leftSection={<IconCloudUp color={theme.colors[theme.primaryColor][4]} height={16} />}
            onClick={() => submitForm()}
          />
        </ResourceViewToolbar>
      <Stack gap="md" p='sm'>
        <TextInput
          label="Build command"
          placeholder="Build command"
          key={form.key('buildCommand')}
          {...form.getInputProps('buildCommand')}
        />
        <Select
          label="Runtime type"
          placeholder="Where your web application will run"
          data={runtimeTypeEnumValueAsList}
          allowDeselect={false}
          key={form.key('runtimeType')}
          {...form.getInputProps('runtimeType')}
        />
        <TextInput
          label="Startup command"
          placeholder="Startup command"
          key={form.key('startupCommand')}
          {...form.getInputProps('startupCommand')}
        />
        <NumberInput
          label="Application port"
          placeholder="Application port"
          key={form.key('port')}
          {...form.getInputProps('port')}
        />
      </Stack>
    </form>
  );
};
