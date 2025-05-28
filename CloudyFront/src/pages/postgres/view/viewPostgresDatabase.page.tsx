import React from 'react';
import {
  IconLock,
  IconPlayerPlayFilled,
  IconPlayerStopFilled,
  IconRestore,
  IconTrash,
} from '@tabler/icons-react';
import { Link, useParams } from 'react-router-dom';
import { Anchor, Divider, Stack, TextInput, useMantineTheme } from '@mantine/core';
import { ResourceViewLayout, ResourceViewPage } from '@/components/ResourceView/ResourceViewLayout';
import { ResourceViewSummary } from '@/components/ResourceView/ResourceViewSummary';
import {
  ResourceViewToolbar,
  ResourceViewToolbarItem,
} from '@/components/ResourceView/ResourceViewToolbar';
import { CloudyIconDatabase, defaultIconStyle } from '@/icons/Resources';
import {
  useGetApiResourceBaseResourceByResourceIdContainerQuery,
  useGetApiResourcePostgresServerResourceByServerIdQuery,
  useGetApiResourcePostgresServerResourceDatabaseByDatabaseIdQuery,
} from '@/services/rtk/cloudyApi';
import { viewResourceOfType } from '@/util/navigation';
import { ConnectionStringsSubpage } from '@/sections/database/postgres/view/ConnectionStringsSubpage';
import { QueryRunnerSubpage } from '@/sections/database/postgres/view/QueryRunnerSubpage';

export function ViewPostgresDatabasePage() {
  const { id: resourceId } = useParams<{ id: string }>();
  const theme = useMantineTheme();
  const { data: resourceBaseData } =
    useGetApiResourcePostgresServerResourceDatabaseByDatabaseIdQuery({
      databaseId: resourceId || '',
    });
  const { data: containerStatus } = useGetApiResourceBaseResourceByResourceIdContainerQuery({
    resourceId: resourceBaseData?.serverId || '',
  });

  return (
    <ResourceViewLayout
      title={
        <>
          <CloudyIconDatabase style={{ ...defaultIconStyle, marginRight: '4px' }} />
          {resourceBaseData?.name || 'Loading'}
        </>
      }
      subtitle={
        <>
          on server{' '}
          <Anchor
            component={Link}
            to={viewResourceOfType('PostgresServerResource', resourceBaseData?.serverId || '')}
          >
            {resourceBaseData?.serverName}
          </Anchor>
          , in{' '}
          <Anchor
            component={Link}
            to={viewResourceOfType('ResourceGroup', resourceBaseData?.resourceGroupId || '')}
          >
            {resourceBaseData?.resourceGroupName}
          </Anchor>
        </>
      }
    >
      <ResourceViewPage title="Overview">
        <ResourceViewToolbar>
          <ResourceViewToolbarItem
            label="Reset admin password (TODO)"
            leftSection={<IconLock color={theme.colors[theme.primaryColor][4]} height={16} />}
            onClick={() => alert('TODO')}
          />
          <ResourceViewToolbarItem
            label="Delete (TODO)"
            leftSection={<IconTrash color={theme.colors[theme.primaryColor][4]} height={16} />}
            onClick={() => alert('TODO')}
          />
        </ResourceViewToolbar>
        <Stack p="sm">
          <ResourceViewSummary
            items={[
              { name: 'Resource group', value: { text: resourceBaseData?.resourceGroupName } },
              { name: 'Database ID', value: { text: resourceBaseData?.id } },
              { name: 'Database name', value: { text: resourceBaseData?.name } },
              { name: 'Admin user', value: { text: resourceBaseData?.adminUsername } },
              {
                name: 'Connection strings',
                value: {
                  text: 'Go to list',
                  link: `${viewResourceOfType('PostgresDatabaseResource', resourceBaseData?.id)}?rpi=1`,
                },
              },
              { name: 'Server ID', value: { text: resourceBaseData?.serverId } },
              {
                name: 'Server Name',
                value: {
                  text: resourceBaseData?.serverName,
                  link: viewResourceOfType('PostgresServerResource', resourceBaseData?.serverId),
                },
              },
              {
                name: 'Server status',
                value: { text: containerStatus?.stateRunning ? 'Active' : 'Stopped' },
              },
              {
                name: 'Created at',
                value: {
                  text:
                    resourceBaseData?.createdAt &&
                    new Date(resourceBaseData.createdAt).toLocaleString(),
                },
              },
            ]}
          />
          <Divider />
        </Stack>
      </ResourceViewPage>
      <ResourceViewPage title="Connection strings">
        <ConnectionStringsSubpage resourceBaseData={resourceBaseData} />
      </ResourceViewPage>
      <ResourceViewPage title="Query runner">
        <QueryRunnerSubpage resourceBaseData={resourceBaseData} />
      </ResourceViewPage>
    </ResourceViewLayout>
  );
}
