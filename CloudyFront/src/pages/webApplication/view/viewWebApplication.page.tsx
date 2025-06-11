import React from 'react';
import {
  IconCpu,
  IconLogs,
  IconNetworkOff,
  IconOutlet,
  IconPlayerPlayFilled,
  IconPlayerStopFilled,
  IconPlugConnected,
  IconRestore,
  IconTerminal2,
  IconTrash,
} from '@tabler/icons-react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { Anchor, Blockquote, Box, Button, Divider, Modal, SimpleGrid, Stack, Text, Title, useMantineTheme } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { MetricChart } from '@/components/Observability/MetricChart';
import { ResourceViewAuditLog } from '@/components/ResourceView/ResourceViewAuditLog';
import { ResourceViewLayout, ResourceViewPage } from '@/components/ResourceView/ResourceViewLayout';
import { ResourceViewLogs } from '@/components/ResourceView/ResourceViewLogs';
import { ResourceViewSummary } from '@/components/ResourceView/ResourceViewSummary';
import { ResourceViewToolbar, ResourceViewToolbarItem } from '@/components/ResourceView/ResourceViewToolbar';
import { CloudyIconWebApplication, defaultIconStyle } from '@/icons/Resources';
import { WebApplicationDeploymentSubpage } from '@/sections/webApplication/view/WebApplicationDeploymentSubpage';
import { WebApplicationEnvironmentSubpage } from '@/sections/webApplication/view/WebApplicationEnvironmentSubpage';
import { WebApplicationNetworkSubpage } from '@/sections/webApplication/view/WebApplicationNetworkSubpage';
import { PredefinedPrometheusQuery, useDeleteApiResourceWebApplicationResourceByIdConfigurationAndConfigurationKeyMutation, useDeleteApiResourceWebApplicationResourceByIdMutation, useGetApiResourceBaseResourceByResourceIdContainerQuery, useGetApiResourceWebApplicationResourceByIdQuery, usePostApiResourceWebApplicationResourceByServerIdContainerActionMutation } from '@/services/rtk/cloudyApi';
import { viewResourceOfType } from '@/util/navigation';


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
  const [deleteResource] = useDeleteApiResourceWebApplicationResourceByIdMutation();
  const [openedDelete, { open: openDelete, close: closeDelete }] = useDisclosure(false);
  const handleDelete = async () => {
    if (!resourceId) return;
    await deleteResource({ id: resourceId }).unwrap();
    closeDelete();
    navigate('/');
  };
  return (
    <ResourceViewLayout
      title={
        <>
          <CloudyIconWebApplication style={{ ...defaultIconStyle, marginRight: '4px' }} />
          {resourceBaseData?.name || 'Loading'}
        </>
      }
      subtitle={
        <>
          in{' '}
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
        <Modal opened={openedDelete} onClose={closeDelete} title="Delete resource">
          <Text>
            Are you sure you want to delete this web application? This action is irreversible and
            your application will not be accessible anymore.
          </Text>
          <Button
            variant="outline"
            color="red"
            onClick={handleDelete}
            style={{ marginTop: '16px' }}
          >
            Yes
          </Button>
          <Button
            variant="default"
            onClick={closeDelete}
            style={{ marginTop: '16px', marginLeft: '8px' }}
          >
            No
          </Button>
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
        <Stack p="sm">
          <ResourceViewSummary
            items={[
              { name: 'Resource group', value: { text: resourceBaseData?.resourceGroupName } },
              { name: 'Web application ID', value: { text: resourceBaseData?.id } },
              { name: 'Web application name', value: { text: resourceBaseData?.name } },
              {
                name: 'Networking',
                value: {
                  text: 'Go to configuration',
                  link: `${viewResourceOfType('WebApplicationResource', resourceBaseData?.id)}?rpi=4`,
                },
              },
              {
                name: 'Created at',
                value: {
                  text:
                    resourceBaseData?.createdAt &&
                    new Date(resourceBaseData?.createdAt).toLocaleString(),
                },
              },
            ]}
          />
          <Box>
            <Title order={3}>Next steps</Title>
            <SimpleGrid
              cols={{ base: 1, lg: 3 }}
              spacing={{ base: 10, xl: 'xl' }}
              verticalSpacing={{ base: 'md', xl: 'xl' }}
            >
              {!resourceBaseData?.virtualNetworks?.length && (
                <Blockquote color="blue" icon={<IconPlugConnected />} mt="xl">
                  Use virtual networks in{' '}
                  <Anchor
                    component={Link}
                    to={`${viewResourceOfType('WebApplicationResource', resourceBaseData?.id)}?rpi=4`}
                  >
                    Networking
                  </Anchor>{' '}
                  section to allow your web application to communicate with other resources such as{' '}
                  <Anchor component={Link} to="/browse/psqldb">
                    Databases
                  </Anchor>{' '}
                  and{' '}
                  <Anchor component={Link} to="/browse/kafka">
                    Message brokers
                  </Anchor>
                  .
                </Blockquote>
              )}
              {!resourceBaseData?.publicProxyConfigurations?.length ? (
                <Blockquote color="yellow" icon={<IconOutlet />} mt="xl">
                  To view your web application, you need to expose it to the public internet. You
                  can do this in the{' '}
                  <Anchor
                    component={Link}
                    to={`${viewResourceOfType('WebApplicationResource', resourceBaseData?.id)}?rpi=4`}
                  >
                    Public endpoints
                  </Anchor>{' '}
                  section.
                </Blockquote>
              ) : (
                <Blockquote color="blue" icon={<IconOutlet />} mt="xl">
                  Your web application is exposed to these public domains:{' '}
                  {resourceBaseData.publicProxyConfigurations
                    .map((config, index) => (
                      <Anchor
                        key={index}
                        href={(config.useHttps ? 'https://' : 'http://') + config.domain}
                        target="_blank"
                        onClick={(e) => e.stopPropagation()}
                      >
                        {config.domain}
                      </Anchor>
                    ))
                    // @ts-ignore cant render such things
                    .reduce((prev, curr) => [prev, ', ', curr])}
                  .
                </Blockquote>
              )}
              {!resourceBaseData?.buildCommand ||
                !resourceBaseData?.startupCommand ||
                (!resourceBaseData?.configuration?.length && (
                  <Blockquote color={!resourceBaseData?.buildCommand || !resourceBaseData?.startupCommand ? 'yellow' : 'blue'} icon={<IconTerminal2 />} mt="xl">
                    To run your web application, you need to set up the build and startup commands
                    in the{' '}
                    <Anchor
                      component={Link}
                      to={`${viewResourceOfType('WebApplicationResource', resourceBaseData?.id)}?rpi=2`}
                    >
                      Deployment
                    </Anchor>{' '}
                    section and environment variables in the{' '}
                    <Anchor
                      component={Link}
                      to={`${viewResourceOfType('WebApplicationResource', resourceBaseData?.id)}?rpi=3`}
                    >
                      Environment variables
                    </Anchor>{' '}
                    section.
                  </Blockquote>
                ))}
                <Blockquote color="blue" icon={<IconLogs />} mt="xl">
                  You can view the build and run logs of your web application in the{' '}
                  <Anchor
                    component={Link}
                    to={`${viewResourceOfType('WebApplicationResource', resourceBaseData?.id)}?rpi=5`}
                  >
                    Logs
                  </Anchor>{' '}
                  section.
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
                Public endpoints requests (per minute):
                <MetricChart
                  resourceId={resourceId || ''}
                  query={PredefinedPrometheusQuery.HttpRequestsCount}
                  range={{}}
                />
              </div>
              <div>
                CPU load (seconds per minute):
                <MetricChart
                  resourceId={resourceId || ''}
                  query={PredefinedPrometheusQuery.GeneralCpuLoad}
                  range={{}}
                />
              </div>
              <div>
                Memory usage (MB):
                <MetricChart
                  resourceId={resourceId || ''}
                  query={PredefinedPrometheusQuery.GeneralMemoryUsage}
                  range={{}}
                />
              </div>
            </SimpleGrid>
          </Box>
        </Stack>
      </ResourceViewPage>
      <ResourceViewPage title="Audit log">
        <ResourceViewAuditLog resourceBaseData={resourceBaseData} />
      </ResourceViewPage>
      <ResourceViewPage title="Deployment">
        {resourceBaseData ? (
          <WebApplicationDeploymentSubpage resourceBaseData={resourceBaseData} />
        ) : (
          'Loading...'
        )}
      </ResourceViewPage>
      <ResourceViewPage title="Environment variables">
        {resourceBaseData ? (
          <WebApplicationEnvironmentSubpage resourceBaseData={resourceBaseData} />
        ) : (
          'Loading...'
        )}
      </ResourceViewPage>
      <ResourceViewPage title="Networking">
        {resourceBaseData ? (
          <WebApplicationNetworkSubpage resourceBaseData={resourceBaseData} />
        ) : (
          'Loading...'
        )}
      </ResourceViewPage>
      <ResourceViewPage title="Logs">
        {resourceBaseData ? <ResourceViewLogs resourceBaseData={resourceBaseData} /> : 'Loading...'}
      </ResourceViewPage>
    </ResourceViewLayout>
  );
}
