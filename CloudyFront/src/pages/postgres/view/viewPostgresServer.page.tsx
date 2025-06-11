import React, { useCallback, useEffect, useMemo, useState } from 'react';
import {
  IconNetworkOff,
  IconPlayerPlayFilled,
  IconPlayerStopFilled,
  IconRestore, IconSettingsPlus,
  IconTrash,
} from '@tabler/icons-react';
import { DataTable } from 'mantine-datatable';
import { Line } from 'react-chartjs-2';
import { Link, useNavigate, useParams } from 'react-router-dom';
import {
  Anchor,
  Blockquote,
  Box,
  Button,
  Divider,
  Modal,
  NavLink,
  SimpleGrid,
  Stack,
  Table,
  Text,
  Title,
  useMantineTheme,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { MetricChart } from '@/components/Observability/MetricChart';
import { ResourceViewAuditLog } from '@/components/ResourceView/ResourceViewAuditLog';
import { ResourceViewLayout, ResourceViewPage } from '@/components/ResourceView/ResourceViewLayout';
import { ResourceViewLogs } from '@/components/ResourceView/ResourceViewLogs';
import { ResourceViewSummary } from '@/components/ResourceView/ResourceViewSummary';
import { ResourceViewToolbar, ResourceViewToolbarItem } from '@/components/ResourceView/ResourceViewToolbar';
import { CloudyIconDatabase, CloudyIconDatabaseServer, defaultIconStyle } from '@/icons/Resources';
import { PostgresExtensionsSubpage } from '@/sections/database/postgres/view/PostgresExtensionsSubpage';
import { PostgresServerNetworkSubpage } from '@/sections/database/postgres/view/PostgresServerNetworkSubpage';
import { PredefinedLokiQuery, PredefinedPrometheusQuery, PrometheusResultDto, useDeleteApiResourcePostgresServerResourceByServerIdMutation, useGetApiResourceBaseResourceByResourceIdAuditLogQuery, useGetApiResourceBaseResourceByResourceIdContainerQuery, useGetApiResourcePostgresServerResourceByServerIdQuery, usePostApiResourceBaseResourceByResourceIdLokiMutation, usePostApiResourceBaseResourceByResourceIdPrometheusMutation, usePostApiResourcePostgresServerResourceByServerIdContainerActionMutation } from '@/services/rtk/cloudyApi';
import { viewResourceOfType } from '@/util/navigation';


export const ViewPostgresServerPage = () => {
  const navigate = useNavigate();
  const theme = useMantineTheme();
  const { id: resourceId } = useParams<{ id: string }>();
  const { data: containerStatus, refetch: refetchContainer } =
    useGetApiResourceBaseResourceByResourceIdContainerQuery({
      resourceId: resourceId || '',
    });
  const { data: resourceBaseData } = useGetApiResourcePostgresServerResourceByServerIdQuery({
    serverId: resourceId || '',
  });

  const [action] = usePostApiResourcePostgresServerResourceByServerIdContainerActionMutation();
  const [deleteResource] = useDeleteApiResourcePostgresServerResourceByServerIdMutation();
  const handleAction = async (actionType: 'start' | 'stop' | 'restart') => {
    if (!containerStatus) return;
    await action({ serverId: resourceId || '', actionId: actionType }).unwrap();
    refetchContainer();
  };
  const [openedDelete, { open:openDelete, close: closeDelete }] = useDisclosure(false);
  const handleDelete = async () => {
    if (!resourceId) return;
    await deleteResource({ serverId: resourceId }).unwrap();
    closeDelete();
    navigate('/');
  }
  return (
    <ResourceViewLayout
      title={
        <>
          <CloudyIconDatabaseServer style={{ ...defaultIconStyle, marginRight: '4px' }} />
          {resourceBaseData?.name || 'Loading'}
        </>
      }
      subtitle={<>in <Anchor component={Link} to={viewResourceOfType('ResourceGroup', resourceBaseData?.resourceGroupId || '')}>{resourceBaseData?.resourceGroupName}</Anchor></>}
    >
      <ResourceViewPage title="Overview">
        <Modal opened={openedDelete} onClose={closeDelete} title="Delete resource">
          <Text>
            Are you sure you want to delete this server? This action is irreversible. All your databases and their data will be lost.
          </Text>
          <Button
            variant="outline"
            color="red"
            onClick={handleDelete}
            style={{ marginTop: '16px' }}>Yes</Button>
          <Button
            variant="default"
            onClick={closeDelete}
            style={{ marginTop: '16px', marginLeft: '8px' }}>No</Button>
        </Modal>
        <ResourceViewToolbar>
          {containerStatus?.stateRunning ? (
            <ResourceViewToolbarItem
              label="Stop"
              leftSection={
                <IconPlayerStopFilled color={theme.colors[theme.primaryColor][4]} height={16} />
              }
              onClick={() => handleAction('stop')}
            />
          ) : (
            <ResourceViewToolbarItem
              label="Play"
              leftSection={
                <IconPlayerPlayFilled color={theme.colors[theme.primaryColor][4]} height={16} />
              }
              onClick={() => handleAction('start')}
            />
          )}
          <ResourceViewToolbarItem
            label="Restart"
            leftSection={<IconRestore color={theme.colors[theme.primaryColor][4]} height={16} />}
            onClick={() => handleAction('restart')}
          />
          <ResourceViewToolbarItem
            label="Delete"
            leftSection={<IconTrash color={theme.colors[theme.primaryColor][4]} height={16} />}
            onClick={openDelete}
          />
        </ResourceViewToolbar>
        <Stack p='sm'>
          <ResourceViewSummary items={[
            { name: 'Resource group', value: { text: resourceBaseData?.resourceGroupName } },
            { name: 'Status', value: { text: containerStatus?.stateRunning ? 'Active' : 'Stopped' } },
            { name: 'Server ID', value: { text: resourceBaseData?.id } },
            { name: 'Server name', value: { text: resourceBaseData?.name } },
            { name: 'Networking', value: { text: 'Go to configuration', link: `${viewResourceOfType('PostgresServerResource', resourceBaseData?.id)}?rpi=4` } },
            { name: 'Databases', value: { text: `${resourceBaseData?.postgresDatabaseResources?.length ?? 0} databases created`, link: `${viewResourceOfType('PostgresServerResource', resourceBaseData?.id)}?rpi=1` } },
            { name: 'Created at', value: { text: resourceBaseData?.createdAt && new Date(resourceBaseData?.createdAt).toLocaleString() } },
          ]} />
          <Divider />
          <Box>
            <Title order={3}>Next steps</Title>
            <SimpleGrid
              cols={{ base: 1, lg: 3 }}
              spacing={{ base: 10, xl: 'xl' }}
              verticalSpacing={{ base: 'md', xl: 'xl' }}
            >
              {!resourceBaseData?.virtualNetworks?.length && (
                <Blockquote color="blue" icon={<IconNetworkOff/>} mt="xl">
                  To connect to this server, you need to add it into some network. Create a virtual network with your server and other resources that need access to it from <Anchor component={Link} to={`${viewResourceOfType('PostgresServerResource', resourceBaseData?.id)}?rpi=5`}>Networking</Anchor>.
                </Blockquote>
              )}
              <Blockquote color="blue" icon={<CloudyIconDatabase />} mt="xl">
                To create, connect to and monitor your server's databases, go to <Anchor component={Link} to={`${viewResourceOfType('PostgresServerResource', resourceBaseData?.id)}?rpi=1`}>Databases</Anchor>.
              </Blockquote>
            </SimpleGrid>
          </Box>
          <Divider />
          <Box>
            <Title order={3}>System health</Title>
            <SimpleGrid
              cols={{ base: 1, xl: 2 }}
              spacing={{ base: 10, xl: 'xl' }}
              verticalSpacing={{ base: 'md', xl: 'xl' }}
            >
              <div>
                SQL Insertions count (per minute):
                <MetricChart resourceId={resourceId || ''} query={PredefinedPrometheusQuery.PostgresEntriesInserted} range={{}} />
              </div>
              <div>
                SQL Selection count (per minute):
                <MetricChart resourceId={resourceId || ''} query={PredefinedPrometheusQuery.PostgresEntriesReturned} range={{}} />
              </div>
              <div>
                CPU load (per minute):
                <MetricChart resourceId={resourceId || ''} query={PredefinedPrometheusQuery.GeneralCpuLoad} range={{}} />
              </div>
              <div>
                Memory usage (MB):
                <MetricChart resourceId={resourceId || ''} query={PredefinedPrometheusQuery.GeneralMemoryUsage} range={{}} />
              </div>
            </SimpleGrid>
          </Box>
        </Stack>
      </ResourceViewPage>
      <ResourceViewPage title="Databases">
        <ResourceViewToolbar>
          <ResourceViewToolbarItem
            label="Create new database"
            leftSection={<CloudyIconDatabase color={theme.colors[theme.primaryColor][4]} height={16} />}
            onClick={() => navigate(`/postgres/new/database?serverId=${resourceBaseData?.id}`)}
          />
        </ResourceViewToolbar>
        <DataTable
          borderRadius="sm"
          withColumnBorders
          withTableBorder={false}
          striped
          highlightOnHover
          records={resourceBaseData?.postgresDatabaseResources || []}
          columns={[
            { accessor: 'databaseName', title: 'Database name' },
            { accessor: 'adminUsername', title: 'Admin username' },
            { accessor: 'createdAt', title: 'Created at', render: ({createdAt: date}) => date && new Date(date.endsWith('Z') ? date : `${date}Z`).toLocaleString() },
            { accessor: 'updatedAt', title: 'Updated at', render: ({updatedAt: date}) => date && new Date(date.endsWith('Z') ? date : `${date}Z`).toLocaleString() },
          ]}
          onRowClick={({ record }) => navigate(viewResourceOfType('PostgresDatabaseResource', record.id))}
        />
      </ResourceViewPage>
      <ResourceViewPage title="Audit log">
        <ResourceViewAuditLog resourceBaseData={resourceBaseData} />
      </ResourceViewPage>
      <ResourceViewPage title="Backup">Soon</ResourceViewPage>
      <ResourceViewPage title="Networking">
        {resourceBaseData ? <PostgresServerNetworkSubpage resourceBaseData={resourceBaseData} /> : 'Loading...'}
      </ResourceViewPage>
      <ResourceViewPage title="Logs">
        { resourceBaseData ? <ResourceViewLogs resourceBaseData={resourceBaseData} /> : 'Loading...' }
      </ResourceViewPage>
    </ResourceViewLayout>
  );
};
