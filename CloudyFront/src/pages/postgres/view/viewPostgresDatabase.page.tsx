import React from 'react';
import {
  IconBrandSpeedtest, IconCpu,
  IconLock,
  IconNetworkOff, IconOutlet,
  IconPlayerPlayFilled,
  IconPlayerStopFilled,
  IconRestore, IconSettingsPlus, IconTerminal, IconTerminal2,
  IconTrash,
} from '@tabler/icons-react';
import { Link, useParams } from 'react-router-dom';
import { Anchor, Blockquote, Box, Divider, SimpleGrid, Stack, TextInput, Title, useMantineTheme } from '@mantine/core';
import { ResourceViewLayout, ResourceViewPage } from '@/components/ResourceView/ResourceViewLayout';
import { ResourceViewSummary } from '@/components/ResourceView/ResourceViewSummary';
import { ResourceViewToolbar, ResourceViewToolbarItem } from '@/components/ResourceView/ResourceViewToolbar';
import { CloudyIconDatabase, defaultIconStyle } from '@/icons/Resources';
import { CustomPerformanceQueryRunnerSubpage } from '@/sections/database/postgres/view/CustomPerformanceQueryRunnerSubpage';
import { PostgresConnectionStringsSubpage } from '@/sections/database/postgres/view/PostgresConnectionStringsSubpage';
import { QueryRunnerSubpage } from '@/sections/database/postgres/view/QueryRunnerSubpage';
import { useGetApiResourceBaseResourceByResourceIdContainerQuery, useGetApiResourcePostgresServerResourceByServerIdQuery, useGetApiResourcePostgresServerResourceDatabaseByDatabaseIdQuery } from '@/services/rtk/cloudyApi';
import { viewResourceOfType } from '@/util/navigation';
import { PostgresExtensionsSubpage } from '@/sections/database/postgres/view/PostgresExtensionsSubpage';


export function ViewPostgresDatabasePage() {
  const { id: resourceId } = useParams<{ id: string }>();
  const theme = useMantineTheme();
  const { data: resourceBaseData } =
    useGetApiResourcePostgresServerResourceDatabaseByDatabaseIdQuery({
      databaseId: resourceId || '',
    });
  const {data: serverIfAccessibleData } = useGetApiResourcePostgresServerResourceByServerIdQuery({
    serverId: resourceBaseData?.serverId || ''
  })
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
          <Box>
            <Title order={3}>Next steps</Title>
            <SimpleGrid
              cols={{ base: 1, lg: 3 }}
              spacing={{ base: 10, xl: 'xl' }}
              verticalSpacing={{ base: 'md', xl: 'xl' }}
            >
              {!serverIfAccessibleData?.virtualNetworks?.length && (
                <Blockquote color="blue" icon={<IconNetworkOff/>} mt="xl">
                  To connect to this server, you need to add it into some network. Create a virtual network with your server and other resources that need access to it from <Anchor component={Link} to={`${viewResourceOfType('PostgresServerResource', serverIfAccessibleData?.id)}?rpi=5`}>Server networking</Anchor>.
                </Blockquote>
              )}
              <Blockquote color="blue" icon={<IconOutlet />} mt="xl">
                To connect to this database, you can use the connection strings available in the <Anchor component={Link} to={`${viewResourceOfType('PostgresDatabaseResource', resourceBaseData?.id)}?rpi=1`}>Connection strings</Anchor> section.
              </Blockquote>
              <Blockquote color="yellow" icon={<IconCpu />} mt="xl">
                Inspect which queries are taking the most time and resources in the <Anchor component={Link} to={`${viewResourceOfType('PostgresDatabaseResource', resourceBaseData?.id)}?rpi=3`}>Inspect performance</Anchor> section.
              </Blockquote>
              <Blockquote color="blue" icon={<IconTerminal2 />} mt="xl">
                Run queries against this database without exposing it to the public internet in the <Anchor component={Link} to={`${viewResourceOfType('PostgresDatabaseResource', resourceBaseData?.id)}?rpi=2`}>Query runner</Anchor> section.
              </Blockquote>
              <Blockquote color="blue" icon={<IconSettingsPlus />} mt="xl">
                To extend your postgres database with extensions, such as postgis or vector, go to <Anchor component={Link} to={`${viewResourceOfType('PostgresDatabaseResource', resourceBaseData?.id)}?rpi=4`}>Extensions</Anchor>.
              </Blockquote>
            </SimpleGrid>
          </Box>
          <Divider />

        </Stack>
      </ResourceViewPage>
      <ResourceViewPage title="Connection strings">
        <PostgresConnectionStringsSubpage resourceBaseData={resourceBaseData} />
      </ResourceViewPage>
      <ResourceViewPage title="Query runner">
        <QueryRunnerSubpage resourceBaseData={resourceBaseData} />
      </ResourceViewPage>
      <ResourceViewPage title="Inspect performace">
        {resourceBaseData ? <CustomPerformanceQueryRunnerSubpage resourceBaseData={resourceBaseData} /> : 'Loading...'}
      </ResourceViewPage>
      <ResourceViewPage title="Extensions">
        {resourceBaseData ? <PostgresExtensionsSubpage resourceBaseData={resourceBaseData} /> : 'Loading...'}
      </ResourceViewPage>
    </ResourceViewLayout>
  );
}
