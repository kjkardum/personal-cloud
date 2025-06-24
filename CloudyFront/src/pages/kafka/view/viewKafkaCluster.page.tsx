import React from 'react';
import { IconPlayerPlayFilled, IconPlayerStopFilled, IconRestore, IconTrash } from '@tabler/icons-react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import {
  Anchor,
  Box,
  Button,
  Divider,
  Modal,
  SimpleGrid,
  Stack,
  Text,
  Title,
  useMantineTheme,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { MetricChart } from '@/components/Observability/MetricChart';
import { ResourceViewLayout, ResourceViewPage } from '@/components/ResourceView/ResourceViewLayout';
import { ResourceViewToolbar, ResourceViewToolbarItem } from '@/components/ResourceView/ResourceViewToolbar';
import {
  CloudyIconKafkaResource,
  defaultIconStyle,
} from '@/icons/Resources';
import {
  PredefinedPrometheusQuery,
  useDeleteApiResourceKafkaClusterResourceByServerIdMutation,
  useGetApiResourceBaseResourceByResourceIdContainerQuery,
  useGetApiResourceKafkaClusterResourceByServerIdQuery, useGetApiResourceKafkaClusterResourceByServerIdTopicsQuery,
  usePostApiResourceKafkaClusterResourceByServerIdContainerActionMutation,
} from '@/services/rtk/cloudyApi';
import { viewResourceOfType } from '@/util/navigation';
import { ResourceViewSummary } from '@/components/ResourceView/ResourceViewSummary';
import { ResourceViewAuditLog } from '@/components/ResourceView/ResourceViewAuditLog';
import { TypeToIcon, TypeToText } from '@/util/typeToDisplay';
import { DataTable } from 'mantine-datatable';
import { KafkaTopicsSubpage } from '@/sections/messageBroker/kafka/view/KafkaTopicsSubpage';
import { KafkaNetworkSubpage } from '@/sections/messageBroker/kafka/view/KafkaNetworkSubpage';
import { ResourceViewLogs } from '@/components/ResourceView/ResourceViewLogs';
import { DockerNamingHelper } from '@/util/dockerNamingHelper';

export const ViewKafkaClusterPage = () => {
  const navigate = useNavigate();
  const theme = useMantineTheme();
  const { id: resourceId } = useParams<{ id: string }>();
  const { data: containerStatus, refetch: refetchContainer } =
    useGetApiResourceBaseResourceByResourceIdContainerQuery({
      resourceId: resourceId || '',
    });
  const { data: resourceBaseData } = useGetApiResourceKafkaClusterResourceByServerIdQuery({
    serverId: resourceId || '',
  });
  const [action] = usePostApiResourceKafkaClusterResourceByServerIdContainerActionMutation();
  const [deleteResource, {isLoading: isDeleting}] = useDeleteApiResourceKafkaClusterResourceByServerIdMutation();
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
          <CloudyIconKafkaResource style={{ ...defaultIconStyle, marginRight: '4px' }} />
          {resourceBaseData?.name || 'Loading'}
        </>
      }
      subtitle={<>in <Anchor component={Link} to={viewResourceOfType('ResourceGroup', resourceBaseData?.resourceGroupId || '')}>{resourceBaseData?.resourceGroupName}</Anchor></>}
    >
      <ResourceViewPage title="Overview">
        <Modal opened={openedDelete} onClose={closeDelete} title="Delete resource">
          <Text>
            Are you sure you want to delete this kafka cluster? This action is irreversible. All your databases and their data will be lost.
          </Text>
          <Button
            loading={isDeleting}
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
            { name: 'Kafka host', value: { text: `${DockerNamingHelper.getContainerName(resourceBaseData?.id || '')}:${9092}` } },
            { name: 'Server name', value: { text: resourceBaseData?.name } },
            { name: 'Networking', value: { text: 'Go to configuration', link: `${viewResourceOfType('KafkaClusterResource', resourceBaseData?.id)}?rpi=3` } },
            { name: 'Created at', value: { text: resourceBaseData?.createdAt && new Date(resourceBaseData?.createdAt).toLocaleString() } },
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
      <ResourceViewPage title="Audit log">
        <ResourceViewAuditLog resourceBaseData={resourceBaseData} />
      </ResourceViewPage>
      <ResourceViewPage title="Topics">
        <KafkaTopicsSubpage resourceId={resourceId || ''} />
      </ResourceViewPage>
      <ResourceViewPage title="Networking">
        {resourceBaseData ? <KafkaNetworkSubpage resourceBaseData={resourceBaseData} /> : 'Loading...'}
      </ResourceViewPage>
      <ResourceViewPage title="Logs">
        { resourceBaseData ? <ResourceViewLogs resourceBaseData={resourceBaseData} /> : 'Loading...' }
      </ResourceViewPage>
    </ResourceViewLayout>
  );
};
