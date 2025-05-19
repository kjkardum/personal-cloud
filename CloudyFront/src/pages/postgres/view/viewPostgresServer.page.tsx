import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { IconPlayerPlayFilled, IconPlayerStopFilled, IconRestore, IconTrash } from '@tabler/icons-react';
import { DataTable } from 'mantine-datatable';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { Box, Button, Divider, Modal, NavLink, Stack, Table, Text, Title, useMantineTheme } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { ResourceViewLayout, ResourceViewPage } from '@/components/ResourceView/ResourceViewLayout';
import { ResourceViewToolbar, ResourceViewToolbarItem } from '@/components/ResourceView/ResourceViewToolbar';
import { CloudyIconDatabase, defaultIconStyle } from '@/icons/Resources';

import {
  PrometheusResultDto,
  useDeleteApiResourcePostgresServerResourceByServerIdMutation,
  useGetApiResourceBaseResourceByResourceIdAuditLogQuery,
  useGetApiResourceBaseResourceByResourceIdContainerQuery,
  useGetApiResourcePostgresServerResourceByServerIdQuery,
  useGetApiResourceResourceGroupedResourceByResourceIdQuery,
  usePostApiResourceBaseResourceByResourceIdPrometheusMutation,
  usePostApiResourcePostgresServerResourceByServerIdContainerActionMutation,
} from '@/services/rtk/cloudyApi';
import { Line } from 'react-chartjs-2';

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
  const [getMetrics] = usePostApiResourceBaseResourceByResourceIdPrometheusMutation();
  const [metricsData, setMetricsData] = useState<PrometheusResultDto | undefined>(undefined);
  useEffect(() => {
    if (!resourceId) return;
    const now = new Date();
    const hourAgo = new Date(now.getTime() - 60 * 60 * 1000);
    getMetrics({resourceId, queryPrometheusQuery: {
      query: "PostgresProcessesCount",
      start: hourAgo.toISOString(),
      end: now.toISOString(),
      step: "15s",
      }}).unwrap().then((metrics) => {
      setMetricsData(metrics);
    });
  }, [resourceId]);
  const customPrometheusReq = useCallback((start, end, step) => {
    return getMetrics({
      resourceId: resourceId || '',
      queryPrometheusQuery: {
        query: "PostgresProcessesCount",
        start, end, step: String(step)
      }
    }).unwrap().then((metrics) => metrics.data)
  }, [resourceId, getMetrics]);
  const memoedOptionsPrometheusChart = useMemo(()=>({
    plugins: {
      'datasource-prometheus': {
        query: customPrometheusReq,
        timeRange: {
          type: 'relative',
          start: -1 * 60 * 60 * 1000, // 1h ago
          end: 0,   // now
        },
      },
    }}), [customPrometheusReq]);
  const memoedDummyDataPrometheusChart = useMemo(()=>({labels: [], datasets: []}), []);
  const [auditPage, setAuditPage] = useState(1);
  const { data: resourceAuditLogPaginated } = useGetApiResourceBaseResourceByResourceIdAuditLogQuery({
    resourceId: resourceId || '',
    page: auditPage,
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
          <CloudyIconDatabase style={{ ...defaultIconStyle, marginRight: '4px' }} />
          {resourceBaseData?.name || 'Loading'}
        </>
      }
      subtitle={`in ${resourceBaseData?.resourceGroupName}`}
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
          <Box>
            <Title order={3}>General information</Title>
            <Table withRowBorders={false}>
              <tbody>
                <tr>
                  <td>Resource group</td>
                  <td>{resourceBaseData?.resourceGroupName}</td>
                </tr>
                <tr>
                  <td>Status</td>
                  <td>{containerStatus?.stateRunning ? 'Active' : 'Stopped'}</td>
                </tr>
                <tr>
                  <td>Server ID</td>
                  <td>{resourceBaseData?.id}</td>
                </tr>
                <tr>
                  <td>Server name</td>
                  <td>{resourceBaseData?.name}</td>
                </tr>
                <tr>
                  <td>Networking</td>
                  <td><NavLink component={Link} replace={true} to={`/postgres/view/server/${resourceBaseData?.id}?rpi=4`} label='Go to configuration' p={0} c={theme.colors[theme.primaryColor][4]}/></td>
                </tr>
                <tr>
                  <td>Databases</td>
                  <td><NavLink component={Link} replace={true} to={`/postgres/view/server/${resourceBaseData?.id}?rpi=1`} label={`${resourceBaseData?.postgresDatabaseResources.length ?? 0} databases created`} p={0} c={theme.colors[theme.primaryColor][4]}/></td>
                </tr>
                <tr>
                  <td>Created at</td>
                  <td>{new Date(resourceBaseData?.createdAt).toLocaleString()}</td>
                </tr>
              </tbody>
            </Table>
          </Box>
          <Divider />
          <Box>
            <Title order={3}>System health</Title>
            <Line data={memoedDummyDataPrometheusChart} options={memoedOptionsPrometheusChart} />
            <pre>{JSON.stringify(metricsData, null, 2)}</pre>
          </Box>
        </Stack>
      </ResourceViewPage>
      <ResourceViewPage title="Databases">
        <ResourceViewToolbar>
          <ResourceViewToolbarItem
            label="Create new database"
            leftSection={<CloudyIconDatabase color={theme.colors[theme.primaryColor][4]} height={16} />}
            onClick={() => navigate(`/postgres/new/database?serverId=${resourceBaseData.id}`)}
          />
        </ResourceViewToolbar>
        <DataTable
          borderRadius="sm"
          withColumnBorders
          striped
          highlightOnHover
          records={resourceBaseData?.postgresDatabaseResources || []}
          columns={[
            { accessor: 'databaseName', title: 'Database name' },
            { accessor: 'adminUsername', title: 'Admin username' },
            { accessor: 'createdAt', title: 'Created at', render: ({createdAt: date}) => new Date(date.endsWith('Z') ? date : date + 'Z').toLocaleString() },
            { accessor: 'updatedAt', title: 'Updated at', render: ({updatedAt: date}) => new Date(date.endsWith('Z') ? date : date + 'Z').toLocaleString() },
          ]}
          onRowClick={({ record }) => alert(JSON.stringify(record))}
        />
      </ResourceViewPage>
      <ResourceViewPage title="Audit log">
        <DataTable
          borderRadius="sm"
          withColumnBorders
          striped
          highlightOnHover
          records={resourceAuditLogPaginated?.data || []}
          totalRecords={resourceAuditLogPaginated?.totalCount || 0}
          recordsPerPage={resourceAuditLogPaginated?.pageSize || 0}
          page={auditPage}
          onPageChange={(page) => setAuditPage(page)}
          columns={[
            { accessor: 'actionDisplayText', title: 'Action' },
            { accessor: 'timestamp', title: 'Timestamp', render: ({timestamp: date}) => new Date(date.endsWith('Z') ? date : date + 'Z').toLocaleString() },
          ]}
          onRowClick={({ record }) => alert(JSON.stringify(record))}
        />
      </ResourceViewPage>
      <ResourceViewPage title="Backup">Soon</ResourceViewPage>
      <ResourceViewPage title="Networking">Soon</ResourceViewPage>
      <ResourceViewPage title="Logs">Soon</ResourceViewPage>
    </ResourceViewLayout>
  );
};
