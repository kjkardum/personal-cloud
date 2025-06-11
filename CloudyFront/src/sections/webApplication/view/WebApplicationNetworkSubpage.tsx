import React, { useCallback, useState } from 'react';
import { IconCloudNetwork, IconExternalLink, IconPlus, IconTopologyRing2, IconTrash } from '@tabler/icons-react';
import { DataTable } from 'mantine-datatable';
import { useNavigate } from 'react-router-dom';
import {
  Anchor,
  Box,
  Button,
  Drawer,
  Group,
  Modal,
  Stack,
  Tabs,
  Text,
  TextInput,
  useMantineTheme,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { ResourceViewToolbar, ResourceViewToolbarItem } from '@/components/ResourceView/ResourceViewToolbar';
import { ReverseProxyConnectDrawer } from '@/components/VirtualNetworks/ReverseProxyConnectDrawer';
import { VirtualNetworkConnectDrawer } from '@/components/VirtualNetworks/VirtualNetworkConnectDrawer';
import { CloudyIconNetwork, CloudyIconVirtualNetworkResource } from '@/icons/Resources';
import { useGetApiResourceWebApplicationResourceByIdQuery, usePostApiResourceReverseProxyDisconnectByResourceIdMutation, WebApplicationResourceDto } from '@/services/rtk/cloudyApi';
import { viewResourceOfType } from '@/util/navigation';


export const WebApplicationNetworkSubpage = ({
  resourceBaseData,
}: {
  resourceBaseData: WebApplicationResourceDto;
}) => {
  const theme = useMantineTheme();
  const navigate = useNavigate();
  const { refetch } = useGetApiResourceWebApplicationResourceByIdQuery({ id: resourceBaseData.id });
  const [
    openedVirtualNetworkDrawer,
    { open: openVirtualNetworkDrawer, close: closeVirtualNetworkDrawer },
  ] = useDisclosure(false);
  const handleCloseVirtualNetworkDrawer = useCallback(() => {
    closeVirtualNetworkDrawer();
    refetch();
  }, [refetch, closeVirtualNetworkDrawer]);
  const [
    openedReverseProxyDrawer,
    { open: openReverseProxyDrawer, close: closeReverseProxyDrawer },
  ] = useDisclosure(false);
  const handleCloseReverseProxyDrawer = useCallback(() => {
    closeReverseProxyDrawer();
    refetch();
  }, [refetch, closeReverseProxyDrawer]);

  const [selectedConnectionId, setSelectedConnectionId] = useState<string | null>(null);
  const [openedDeletePublicConnection, { open: openDeletePublicConnection, close: closeDeletePublicConnection }] = useDisclosure(false);
  const [deletePublicConnectionMutation, { isLoading: isDeletingPublicConnection }] = usePostApiResourceReverseProxyDisconnectByResourceIdMutation();
  const handleOpenDeletePublicConnection = useCallback((connectionId: string) => {
    setSelectedConnectionId(connectionId);
    openDeletePublicConnection();
  }, [openDeletePublicConnection, setSelectedConnectionId]);
  const handleDeletePublicConnection = useCallback(async () => {
    if (!resourceBaseData.id || !selectedConnectionId) { return; }
    await deletePublicConnectionMutation({ resourceId: resourceBaseData.id, disconnectReverseProxyCommand: {connectionId: selectedConnectionId} }).unwrap();
    closeDeletePublicConnection();
    refetch();
  }, [deletePublicConnectionMutation, resourceBaseData.id, closeDeletePublicConnection, refetch, selectedConnectionId]);
  return (
    <Tabs defaultValue="public">
      <Tabs.List>
        <Tabs.Tab h={40} value="public" leftSection={<CloudyIconNetwork />}>
          Public access
        </Tabs.Tab>
        <Tabs.Tab h={40} value="private" leftSection={<CloudyIconVirtualNetworkResource />}>
          Private virtual networks
        </Tabs.Tab>
      </Tabs.List>

      <Tabs.Panel value="public">
        <Modal opened={openedDeletePublicConnection} onClose={closeDeletePublicConnection} title="Delete resource">
          <Text>
            Are you sure you want to delete this public connection? Your resource may no longer be accessible over the internet.
          </Text>
          <Button
            loading={isDeletingPublicConnection}
            variant="outline"
            color="red"
            onClick={handleDeletePublicConnection}
            style={{ marginTop: '16px' }}>Yes</Button>
          <Button
            variant="default"
            onClick={closeDeletePublicConnection}
            style={{ marginTop: '16px', marginLeft: '8px' }}>No</Button>
        </Modal>
        <ReverseProxyConnectDrawer
          opened={openedReverseProxyDrawer}
          onClose={handleCloseReverseProxyDrawer}
          resourceId={resourceBaseData.id}
          resourceType="WebApplicationResource"
          />
        <Stack>
          <ResourceViewToolbar>
            <ResourceViewToolbarItem
              label="Connect public network"
              leftSection={<CloudyIconNetwork color={theme.colors[theme.primaryColor][4]} height={16} />}
              onClick={openReverseProxyDrawer}
            />
          </ResourceViewToolbar>
          <DataTable
            minHeight={400}
            noRecordsText="No public connections yet"
            flex={1}
            borderRadius="sm"
            withTableBorder={false}
            withColumnBorders
            striped
            highlightOnHover
            records={resourceBaseData?.publicProxyConfigurations || []}
            columns={[
              { accessor: 'domain', title: 'Domain', render: ({ domain, useHttps }) => (<Anchor href={(useHttps ? "https://" : "http://") + domain} target="_blank"><Group gap={4}>{domain}<IconExternalLink size={12} /></Group></Anchor>) },
              { accessor: 'useHttps', title: 'HTTPS', render: ({ useHttps }) => (useHttps ? 'Yes' : 'No') },
              { accessor: 'id', title: 'Id' },
              {
                accessor: 'actions',
                title: 'Actions',
                render: ({ id }) => (
                  <Anchor
                    component="button"
                    onClick={() => handleOpenDeletePublicConnection(id!)}
                  >
                    <IconTrash size={16} />
                  </Anchor>
                ),
              }
            ]}
          />
        </Stack>
      </Tabs.Panel>

      <Tabs.Panel value="private">
        <VirtualNetworkConnectDrawer
          opened={openedVirtualNetworkDrawer}
          onClose={handleCloseVirtualNetworkDrawer}
          resourceId={resourceBaseData.id}
        />
        <Stack>
          <ResourceViewToolbar>
            <ResourceViewToolbarItem
              label="Join virtual network"
              leftSection={<IconPlus color={theme.colors[theme.primaryColor][4]} height={16} />}
              onClick={openVirtualNetworkDrawer}
            />
          </ResourceViewToolbar>
          <DataTable
            minHeight={400}
            noRecordsText="No virtual networks joined yet"
            flex={1}
            borderRadius="sm"
            withTableBorder={false}
            withColumnBorders
            striped
            highlightOnHover
            records={resourceBaseData?.virtualNetworks || []}
            columns={[
              { accessor: 'name', title: 'Virtual network' },
              { accessor: 'id', title: 'Id' },
            ]}
            onRowClick={({ record }) =>
              navigate(viewResourceOfType('VirtualNetworkResource', record.id))
            }
          />
        </Stack>
      </Tabs.Panel>
    </Tabs>
  );
};
