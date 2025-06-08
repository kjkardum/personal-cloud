import React from "react";
import { IconPlayerPlayFilled, IconPlayerStopFilled, IconRestore, IconTrash } from '@tabler/icons-react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { Anchor, Box, Button, Divider, Modal, SimpleGrid, Stack, Text, Title, useMantineTheme } from '@mantine/core';
import { ResourceViewAuditLog } from '@/components/ResourceView/ResourceViewAuditLog';
import { ResourceViewLayout, ResourceViewPage } from '@/components/ResourceView/ResourceViewLayout';
import { ResourceViewSummary } from '@/components/ResourceView/ResourceViewSummary';
import { ResourceViewToolbar, ResourceViewToolbarItem } from '@/components/ResourceView/ResourceViewToolbar';
import { CloudyIconWebApplication, defaultIconStyle } from '@/icons/Resources';
import {
  PredefinedPrometheusQuery, useDeleteApiResourceWebApplicationResourceByIdConfigurationAndConfigurationKeyMutation,
  useGetApiResourceBaseResourceByResourceIdContainerQuery,
  useGetApiResourceWebApplicationResourceByIdQuery,
  usePostApiResourceWebApplicationResourceByServerIdContainerActionMutation,
} from '@/services/rtk/cloudyApi';
import { viewResourceOfType } from '@/util/navigation';
import { WebApplicationDeploymentSubpage } from '@/sections/webApplication/view/WebApplicationDeploymentSubpage';
import { WebApplicationEnvironmentSubpage } from '@/sections/webApplication/view/WebApplicationEnvironmentSubpage';
import { WebApplicationNetworkSubpage } from '@/sections/webApplication/view/WebApplicationNetworkSubpage';
import { ResourceViewLogs } from '@/components/ResourceView/ResourceViewLogs';
import { MetricChart } from '@/components/Observability/MetricChart';
import { useDisclosure } from '@mantine/hooks';


export function ViewWebApplicationPage() {
  const navigate = useNavigate();
  const { id: resourceId } = useParams<{ id: string }>();
  const theme = useMantineTheme();
  const { data: resourceBaseData } = useGetApiResourceWebApplicationResourceByIdQuery({
    id: resourceId || '',
  });

  const { data: containerStatus, refetch: refetchContainer } =
    useGetApiResourceBaseResourceByResourceIdContainerQuery({
      resourceId: resourceId || '',
    });
  const [action] = usePostApiResourceWebApplicationResourceByServerIdContainerActionMutation();
  const handleAction = async (actionType: 'start' | 'stop' | 'restart') => {
    if (!containerStatus) return;
    await action({ serverId: resourceId || '', actionId: actionType }).unwrap();
    refetchContainer();
  };
  const [deleteResource] = useDeleteApiResourceWebApplicationResourceByIdConfigurationAndConfigurationKeyMutation();
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
          <CloudyIconWebApplication style={{ ...defaultIconStyle, marginRight: '4px' }} />
          {resourceBaseData?.name || 'Loading'}
        </>
      }
      subtitle={<>in <Anchor component={Link} to={viewResourceOfType('ResourceGroup', resourceBaseData?.resourceGroupId || '')}>{resourceBaseData?.resourceGroupName}</Anchor></>}
    >
      <ResourceViewPage title="Overview">
        <Modal opened={openedDelete} onClose={closeDelete} title="Delete resource">
          <Text>
            Are you sure you want to delete this web application? This action is irreversible and your application will not be accessible anymore.
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
            { name: 'Web application ID', value: { text: resourceBaseData?.id } },
            { name: 'Web application name', value: { text: resourceBaseData?.name } },
            { name: 'Networking', value: { text: 'Go to configuration', link: `${viewResourceOfType('WebApplicationResource', resourceBaseData?.id)}?rpi=4` } },
            { name: 'Created at', value: { text: new Date(resourceBaseData?.createdAt).toLocaleString() } },
          ]} />
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
                <MetricChart resourceId={resourceId || ''} query={PredefinedPrometheusQuery.HttpRequestsCount} range={{}} />
              </div>
              <div>
                CPU load (seconds per minute):
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
      <ResourceViewPage title="Audit log">
        <ResourceViewAuditLog resourceBaseData={resourceBaseData} />
      </ResourceViewPage>
      <ResourceViewPage title="Deployment">
        {resourceBaseData ? <WebApplicationDeploymentSubpage resourceBaseData={resourceBaseData} /> : 'Loading...'}
      </ResourceViewPage>
      <ResourceViewPage title="Environment variables">
        {resourceBaseData ? <WebApplicationEnvironmentSubpage resourceBaseData={resourceBaseData} /> : 'Loading...'}
      </ResourceViewPage>
      <ResourceViewPage title="Networking">
        {resourceBaseData ? <WebApplicationNetworkSubpage resourceBaseData={resourceBaseData} /> : 'Loading...'}
      </ResourceViewPage>
      <ResourceViewPage title="Logs">
        { resourceBaseData ? <ResourceViewLogs resourceBaseData={resourceBaseData} /> : 'Loading...' }
      </ResourceViewPage>
    </ResourceViewLayout>
  );
}
