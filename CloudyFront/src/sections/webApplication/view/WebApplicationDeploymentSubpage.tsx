import { useCallback } from 'react';
import { IconCloudUp } from '@tabler/icons-react';
import { z } from 'zod';
import { NumberInput, Select, Stack, TextInput, useMantineTheme } from '@mantine/core';
import { useForm, zodResolver } from '@mantine/form';
import { ResourceViewToolbar, ResourceViewToolbarItem } from '@/components/ResourceView/ResourceViewToolbar';
import { UpdateWebApplicationDeploymentConfigurationCommand, usePutApiResourceWebApplicationResourceByIdDeploymentConfigurationMutation, WebApplicationResourceDto, WebApplicationRuntimeType } from '@/services/rtk/cloudyApi';


type runtimeTypeEnum = { [enumValue in WebApplicationRuntimeType]: {label: string, placeholderBuild: string, placeholderRun: string} };
const runtimeTypeEnumOptions: runtimeTypeEnum = {
  [WebApplicationRuntimeType.DotNet]: {label: '.NET 9', placeholderBuild: 'cd backend && dotnet public -c Release -o out', placeholderRun: 'cd backend/out dotnet YourApp.dll'},
  [WebApplicationRuntimeType.NodeJs]: {label: 'Node.js 20', placeholderBuild: 'yarn install && yarn build', placeholderRun: 'node dist/server.js'},
  [WebApplicationRuntimeType.Python]: {label: 'Python 3.11', placeholderBuild: 'pip install -r requirements.txt && alembic upgrade head', placeholderRun: 'pip install -r requirements.txt && python app.py'},
};

const runtimeTypeEnumValueAsList = Object.entries(runtimeTypeEnumOptions).map(
  ([value, { label }]) => ({
    value: value as WebApplicationRuntimeType,
    label,
  })
);

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
  const latestValues = form.getValues();
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
          placeholder={latestValues.runtimeType ? runtimeTypeEnumOptions[latestValues.runtimeType].placeholderBuild : 'Build command'}
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
          placeholder={latestValues.runtimeType ? runtimeTypeEnumOptions[latestValues.runtimeType].placeholderRun : 'Startup command'}
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
