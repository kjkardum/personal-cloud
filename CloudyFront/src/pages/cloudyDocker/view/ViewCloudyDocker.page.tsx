import React, { useMemo } from 'react';
import { DataTable } from 'mantine-datatable';
import { Link, useNavigate } from 'react-router-dom';
import { GraphCanvas } from 'reagraph';
import {
  Anchor,
  Box,
  Button,
  Divider, Group,
  Modal,
  SimpleGrid,
  Stack,
  Text,
  Title,
  useMantineTheme,
} from '@mantine/core';
import { MetricChart } from '@/components/Observability/MetricChart';
import { ResourceViewAuditLog } from '@/components/ResourceView/ResourceViewAuditLog';
import { ResourceViewLayout, ResourceViewPage } from '@/components/ResourceView/ResourceViewLayout';
import { ResourceViewSummary } from '@/components/ResourceView/ResourceViewSummary';
import { ResourceViewToolbar } from '@/components/ResourceView/ResourceViewToolbar';
import { CloudyIconDocker, defaultIconStyle } from '@/icons/Resources';
import {
  PredefinedPrometheusQuery,
  useGetApiResourceBaseResourceDockerEnvironmentQuery,
  useGetApiResourceBaseResourceQuery,
  useGetApiResourceResourceGroupQuery,
} from '@/services/rtk/cloudyApi';
import { EmptyGuid } from '@/util/guid';
import { IconExternalLink } from '@tabler/icons-react';
import { GrafanaSubpage } from '@/sections/grafana/grafanaSubpage';

export const ViewCloudyDockerPage = () => {
  const navigate = useNavigate();
  const theme = useMantineTheme();
  const { data: dockerEnvironment } = useGetApiResourceBaseResourceDockerEnvironmentQuery();
  const { data: resourceGroupsData } = useGetApiResourceResourceGroupQuery({ page: 1 });
  const { data: resourcesData } = useGetApiResourceBaseResourceQuery({ page: 1 });
  const topology = useMemo(() => {
    //format { points: {id, type}[], links: {sourceId, targetId}[] }
    if (!dockerEnvironment) return { points: [], links: [] };
    //take all containers and networks
    const points = [
      ...(dockerEnvironment.containers?.map((c) => ({
        id: c.containerName ?? '<none-container>',
        type: 'container',
      })) ?? []),
      ...(dockerEnvironment.networks?.map((n) => ({
        id: n.name ?? '<none-network>',
        type: 'network',
      })) ?? []),
    ];
    const networkIds = new Set(
      dockerEnvironment.containers?.flatMap((c) => c.networkIds ?? []) ?? []
    );
    networkIds.forEach((networkId) => {
      if (!points.some((p) => p.id === networkId)) {
        points.push({ id: networkId, type: 'network' });
      }
    });
    const links =
      dockerEnvironment.containers?.flatMap((n) => {
        return (
          n.networkIds?.map((network) => ({
            sourceId: n.containerName ?? '<none-container>',
            targetId: network,
          })) ?? []
        );
      }) ?? [];
    return {
      points: points.map((point) => ({
        id: point.id,
        label: point.id,
        fill: point.type === 'container' ? theme.colors.blue[6] : theme.colors.green[6],
        icon: point.type === 'container'
        ? "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='24' height='24' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' class='icon icon-tabler icons-tabler-outline icon-tabler-box'%3E%3Cpath stroke='none' d='M0 0h24v24H0z' fill='none'/%3E%3Cpath d='M12 3l8 4.5l0 9l-8 4.5l-8 -4.5l0 -9l8 -4.5' /%3E%3Cpath d='M12 12l8 -4.5' /%3E%3Cpath d='M12 12l0 9' /%3E%3Cpath d='M12 12l-8 -4.5' /%3E%3C/svg%3E"
          : "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='24' height='24' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' class='icon icon-tabler icons-tabler-outline icon-tabler-topology-ring-2'%3E%3Cpath stroke='none' d='M0 0h24v24H0z' fill='none'/%3E%3Cpath d='M14 6a2 2 0 1 0 -4 0a2 2 0 0 0 4 0z' /%3E%3Cpath d='M7 18a2 2 0 1 0 -4 0a2 2 0 0 0 4 0z' /%3E%3Cpath d='M21 18a2 2 0 1 0 -4 0a2 2 0 0 0 4 0z' /%3E%3Cpath d='M7 18h10' /%3E%3Cpath d='M18 16l-5 -8' /%3E%3Cpath d='M11 8l-5 8' /%3E%3C/svg%3E"
      })),
      links: links.map((link) => ({
        id: `${link.sourceId}->${link.targetId}`,
        source: link.sourceId,
        target: link.targetId,
      })),
    };
  }, [dockerEnvironment]);
  return (
    <ResourceViewLayout
      title={
        <>
          <CloudyIconDocker style={{ ...defaultIconStyle, marginRight: '4px' }} />
          Cloudy Host
        </>
      }
      subtitle={<>Manage your host</>}
    >
      <ResourceViewPage title="Overview">
        <Stack p="sm">
          <ResourceViewSummary
            items={[
              {
                name: 'Resource groups',
                value: { text: `${resourceGroupsData?.totalCount} groups` },
              },
              { name: 'Resources', value: { text: `${resourcesData?.totalCount} resources` } },
              {
                name: 'Containers',
                value: { text: `${dockerEnvironment?.containers?.length} containers` },
              },
              { name: 'Images', value: { text: `${dockerEnvironment?.images?.length} images` } },
              {
                name: 'Networks',
                value: { text: `${dockerEnvironment?.networks?.length} networks` },
              },
              { name: 'Volumes', value: { text: `${dockerEnvironment?.volumes?.length} volumes` } },
            ]}
          />
          <Divider />
          <Box>
            <Title order={3}>System health</Title>
            <SimpleGrid
              cols={{ base: 1, xl: 2 }}
              spacing={{ base: 10, xl: 'xl' }}
              verticalSpacing={{ base: 'md', xl: 'xl' }}
            >
              <div>
                Public endpoints requests (per minute):
                <MetricChart
                  resourceId={EmptyGuid}
                  query={PredefinedPrometheusQuery.HttpRequestsCount}
                  range={{}}
                />
              </div>
              <div>
                CPU load (seconds per minute):
                <MetricChart
                  resourceId={EmptyGuid}
                  query={PredefinedPrometheusQuery.GeneralCpuLoad}
                  range={{}}
                />
              </div>
              <div>
                Memory usage (MB):
                <MetricChart
                  resourceId={EmptyGuid}
                  query={PredefinedPrometheusQuery.GeneralMemoryUsage}
                  range={{}}
                />
              </div>
            </SimpleGrid>
          </Box>
        </Stack>
      </ResourceViewPage>
      <ResourceViewPage title="Audit log">
        <ResourceViewAuditLog resourceBaseData={{ id: EmptyGuid }} />
      </ResourceViewPage>
      <ResourceViewPage title="Grafana - Logs and metrics">
        <GrafanaSubpage />
      </ResourceViewPage>
      <ResourceViewPage title="Public network access">TODO https and stuff</ResourceViewPage>
      <ResourceViewPage title="Network visualiser">
        <Stack h='100%'>
          <Box w='100%' flex={1} pos='relative'>
            <GraphCanvas nodes={topology.points} edges={topology.links} />
          </Box>
        </Stack>
      </ResourceViewPage>
      <ResourceViewPage title="Containers">
        <DataTable
          borderRadius="sm"
          withTableBorder={false}
          withColumnBorders
          striped
          highlightOnHover
          records={dockerEnvironment?.containers ?? []}
          columns={[
            { accessor: 'containerName', title: 'Name' },
            { accessor: 'containerId', title: 'Container ID' },
            {
              accessor: 'stateRunning',
              title: 'Running',
              render: ({ stateRunning }) => (stateRunning ? 'Yes' : 'No'),
            },
            {
              accessor: 'statePaused',
              title: 'Paused',
              render: ({ statePaused }) => (statePaused ? 'Yes' : 'No'),
            },
            {
              accessor: 'stateRestarting',
              title: 'Restarting',
              render: ({ stateRestarting }) => (stateRestarting ? 'Yes' : 'No'),
            },
            {
              accessor: 'stateError',
              title: 'Error',
              render: ({ stateError }) => stateError || '-',
            },
            {
              accessor: 'stateStartedAt',
              title: 'Started at',
              render: ({ stateStartedAt }) => new Date(stateStartedAt || '').toLocaleString(),
            },
            {
              accessor: 'stateFinishedAt',
              title: 'Finished at',
              render: ({ stateFinishedAt }) => new Date(stateFinishedAt || '').toLocaleString(),
            },
          ]}
        />
      </ResourceViewPage>
      <ResourceViewPage title="Volumes">
        <DataTable
          borderRadius="sm"
          withColumnBorders
          withTableBorder={false}
          striped
          highlightOnHover
          records={dockerEnvironment?.volumes ?? []}
          columns={[
            { accessor: 'name', title: 'Name' },
            {
              accessor: 'createdAt',
              title: 'Created at',
              render: ({ createdAt }) => new Date(createdAt || '').toLocaleString(),
            },
          ]}
        />
      </ResourceViewPage>
      <ResourceViewPage title="Networks">
        <DataTable
          borderRadius="sm"
          withColumnBorders
          withTableBorder={false}
          striped
          highlightOnHover
          records={dockerEnvironment?.networks ?? []}
          columns={[
            { accessor: 'name', title: 'Name' },
            { accessor: 'networkId', title: 'Id' },
            {
              accessor: 'containerIds',
              title: 'Containers',
              render: ({ containerIds }) => `${containerIds?.length ?? 0} containers`,
            },
          ]}
        />
      </ResourceViewPage>
      <ResourceViewPage title="Images">
        <DataTable
          borderRadius="sm"
          withColumnBorders
          withTableBorder={false}
          striped
          highlightOnHover
          records={dockerEnvironment?.images ?? []}
          columns={[
            { accessor: 'tag', title: 'Tag' },
            { accessor: 'imageId', title: 'Id' },
            {
              accessor: 'createdAt',
              title: 'Created at',
              render: ({ createdAt }) => new Date(createdAt || '').toLocaleString(),
            },
            {
              accessor: 'size',
              title: 'Size',
              render: ({ size }) => `${Math.round((size || 0) / (1024 * 1024))} MB`,
            },
          ]}
        />{' '}
      </ResourceViewPage>
    </ResourceViewLayout>
  );
};
