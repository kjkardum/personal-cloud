import { ChangeEvent, useCallback, useMemo, useState } from 'react';
import { IconDeviceFloppy, IconEqual, IconEye, IconPlus, IconTrash } from '@tabler/icons-react';
import { ActionIcon, Grid, Group, Stack, TextInput, useMantineTheme } from '@mantine/core';
import {
  ResourceViewToolbar,
  ResourceViewToolbarItem,
} from '@/components/ResourceView/ResourceViewToolbar';
import {
  useDeleteApiResourceWebApplicationResourceByIdConfigurationAndConfigurationKeyMutation,
  useGetApiResourceWebApplicationResourceByIdQuery,
  usePostApiResourceWebApplicationResourceByIdConfigurationMutation,
  WebApplicationConfigurationEntryDto,
  WebApplicationResourceDto,
} from '@/services/rtk/cloudyApi';

type ConfigEntryItem = WebApplicationConfigurationEntryDto & { visible?: boolean };

export const WebApplicationEnvironmentSubpage = ({
  resourceBaseData,
}: {
  resourceBaseData: WebApplicationResourceDto;
}) => {
  const [upsertWebConfiguration] =
    usePostApiResourceWebApplicationResourceByIdConfigurationMutation();
  const [deleteConfiguration] =
    useDeleteApiResourceWebApplicationResourceByIdConfigurationAndConfigurationKeyMutation();
  const {refetch: refectchBaseData } = useGetApiResourceWebApplicationResourceByIdQuery({id: resourceBaseData.id});
  const [currentConfiguration, setCurrentConfiguration] = useState<ConfigEntryItem[]>(
    resourceBaseData.configuration || []
  );
  const [savingConfiguration, setSavingConfiguration] = useState(false);
  const updateConfigurationItem = (
    index: number,
    key?: string,
    value?: string,
    toggleVisible?: boolean
  ) => {
    setCurrentConfiguration((prev) => {
      const newConfig = [...prev];
      const newItem = { ...newConfig[index] };
      if (key !== undefined) {
        newItem.key = key;
      }
      if (value !== undefined) {
        newItem.value = value;
      }
      if (toggleVisible) {
        newItem.visible = !newItem.visible;
      }
      newConfig[index] = newItem;
      return newConfig;
    });
  };
  const removeConfigurationItem = (index: number) => () => {
    setCurrentConfiguration((prev) => {
      const newConfig = [...prev];
      newConfig.splice(index, 1);
      return newConfig;
    });
  };
  const updateConfigurationKey = (index: number) => (e: ChangeEvent<HTMLInputElement>) =>
    updateConfigurationItem(index, e.target.value, undefined, false);
  const updateConfigurationValue = (index: number) => (e: ChangeEvent<HTMLInputElement>) =>
    updateConfigurationItem(index, undefined, e.target.value, false);
  const toggleVisibility = (index: number) => () =>
    updateConfigurationItem(index, undefined, undefined, true);
  const addConfigurationEntry = () => {
    setCurrentConfiguration((prev) => [...prev, { key: '', value: '', visible: true }]);
  };
  const configurationChanges = useMemo(() => {
    const previous = resourceBaseData.configuration || [];
    const current = currentConfiguration;
    const remove = previous.filter((item) => !current.some((c) => c.key === item.key));
    const upsert = current.filter((item) => {
      const prevItem = previous.find((prev) => prev.key === item.key);
      return !prevItem || prevItem.value !== item.value;
    });
    return { remove, upsert };
  }, [currentConfiguration, resourceBaseData.configuration]);
  const saveConfigurationChanges = useCallback(async () => {
    if (savingConfiguration) return;
    setSavingConfiguration(true);
    try {
      const { remove, upsert } = configurationChanges;
      for (const item of remove) {
        await deleteConfiguration({
          id: resourceBaseData.id,
          configurationKey: item.key ?? '',
        }).unwrap();
      }
      for (const item of upsert) {
        await upsertWebConfiguration({
          id: resourceBaseData.id,
          modifyWebApplicationConfigItemCommand: {
            key: item.key,
            value: item.value,
          },
        }).unwrap();
      }
    } finally {
      refectchBaseData();
      setSavingConfiguration(false);
    }
  }, [configurationChanges, resourceBaseData.id, upsertWebConfiguration, deleteConfiguration]);

  const theme = useMantineTheme();
  return (
    <>
      <ResourceViewToolbar>
        <ResourceViewToolbarItem
          label="Add variable"
          leftSection={<IconPlus color={theme.colors[theme.primaryColor][4]} height={16} />}
          onClick={addConfigurationEntry}
        />
        <ResourceViewToolbarItem
          label="Save changes"
          disabled={!configurationChanges.remove.length && !configurationChanges.upsert.length || savingConfiguration}
          leftSection={<IconDeviceFloppy color={theme.colors[theme.primaryColor][4]} height={16} />}
          onClick={saveConfigurationChanges}
        />
      </ResourceViewToolbar>
      <Stack gap="sm" p="md">
        {currentConfiguration.map(({ key, value, ...rest }, index) => (
          <Grid key={index} align="end">
            <Grid.Col span={4}>
              <TextInput
                label="Key"
                placeholder="(eg. NODE_ENV)"
                value={key}
                onChange={updateConfigurationKey(index)}
              />
            </Grid.Col>
            <Grid.Col span={1} ta="center">
              <IconEqual />
            </Grid.Col>
            <Grid.Col span={4}>
              <TextInput
                label="Value"
                placeholder="(eg. production)"
                type={rest.visible ? 'text' : 'password'}
                value={value}
                onChange={updateConfigurationValue(index)}
              />
            </Grid.Col>
            <Grid.Col span={3}>
              <ActionIcon variant="default">
                <IconEye onClick={toggleVisibility(index)} />
              </ActionIcon>
              <ActionIcon variant="subtle" color="red" ml='sm'>
                <IconTrash onClick={removeConfigurationItem(index)} />
              </ActionIcon>
            </Grid.Col>
          </Grid>
        ))}
      </Stack>
    </>
  );
};
