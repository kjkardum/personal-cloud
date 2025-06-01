import React, { useCallback } from 'react';
import { IconCloudNetwork, IconPlus, IconTopologyRing2 } from '@tabler/icons-react';
import { DataTable } from 'mantine-datatable';
import { Anchor, Box, Drawer, Stack, Tabs, TextInput, useMantineTheme } from '@mantine/core';
import {
  ResourceViewToolbar,
  ResourceViewToolbarItem,
} from '@/components/ResourceView/ResourceViewToolbar';
import { CloudyIconNetwork, CloudyIconVirtualNetworkResource } from '@/icons/Resources';
import { useGetApiResourceWebApplicationResourceByIdQuery, WebApplicationResourceDto } from '@/services/rtk/cloudyApi';
import { useDisclosure } from '@mantine/hooks';
import { VirtualNetworkConnectDrawer } from '@/components/VirtualNetworks/VirtualNetworkConnectDrawer';
import { viewResourceOfType } from '@/util/navigation';
import { useNavigate } from 'react-router-dom';

export const WebApplicationNetworkSubpage = ({
  resourceBaseData,
}: {
  resourceBaseData: WebApplicationResourceDto;
}) => {
  const theme = useMantineTheme();
  const navigate = useNavigate();
  const {refetch} = useGetApiResourceWebApplicationResourceByIdQuery({id: resourceBaseData.id});
  const [
    openedNtworkDrawer, { open: openNetworkDrawer, close: closeNetworkDrawer }] = useDisclosure(false);
  const handleCloseNetworkDrawer = useCallback(()=> {
    closeNetworkDrawer();
    refetch();
  }, [refetch, closeNetworkDrawer]);
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

      <Tabs.Panel value="public">TODO</Tabs.Panel>

      <Tabs.Panel value="private">
        <VirtualNetworkConnectDrawer opened={openedNtworkDrawer} onClose={handleCloseNetworkDrawer} resourceId={resourceBaseData.id} />
        <Stack>
          <ResourceViewToolbar>
            <ResourceViewToolbarItem
              label="Join virtual network TODO"
              leftSection={<IconPlus color={theme.colors[theme.primaryColor][4]} height={16} />}
              onClick={openNetworkDrawer}
            />
          </ResourceViewToolbar>
          <DataTable
            minHeight={400}
            noRecordsText={'No virtual networks joined yet'}
            flex={1}
            borderRadius="sm"
            withColumnBorders
            striped
            highlightOnHover
            records={resourceBaseData?.virtualNetworks || []}
            columns={[
              { accessor: 'name', title: 'Virtual network' },
              { accessor: 'id', title: 'Id' },
            ]}
            onRowClick={({ record }) => navigate(viewResourceOfType('VirtualNetworkResource', record.id))}
          />
        </Stack>
      </Tabs.Panel>
    </Tabs>
  );
};
