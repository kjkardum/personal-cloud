import { Anchor, Box, Drawer, DrawerProps, Stack, TextInput } from '@mantine/core';
import React, { ChangeEvent, useCallback, useState } from 'react';
import { useDebouncedValue } from '@mantine/hooks';
import {
  useGetApiResourceVirtualNetworkResourceQuery, usePostApiResourceVirtualNetworkResourceByIdConnectMutation,
} from '@/services/rtk/cloudyApi';
import { DataTable } from 'mantine-datatable';
import { viewResourceOfType } from '@/util/navigation';
import { Link, useNavigate } from 'react-router-dom';

export const VirtualNetworkConnectDrawer = ({resourceId, ...props}: DrawerProps & {resourceId: string}) => {
  const navigate = useNavigate();
  const [searchValue, setSearchValue] = useState('');
  const [debouncedSearch] = useDebouncedValue(searchValue, 300);
  const handleSetSearchValue = useCallback((e: ChangeEvent<HTMLInputElement>) => setSearchValue(e.target.value), [setSearchValue]);

  const { data, refetch } = useGetApiResourceVirtualNetworkResourceQuery(
    { filterBy: debouncedSearch, }, {refetchOnMountOrArgChange: true});
  const [connectVirtualNetworkResource] = usePostApiResourceVirtualNetworkResourceByIdConnectMutation();

  const attachNetwork = useCallback(async (id: string) => {
    await connectVirtualNetworkResource({id, connectVirtualNetworkCommand: {resourceId}}).unwrap();
    props.onClose();
  }, [connectVirtualNetworkResource, resourceId]);
  return (
    <Drawer offset={8} radius="sm" position="right" size='xl' title="Add virtual network" {...props}>
      <Stack gap='md'>
        <Box>
          <TextInput
            label="Virtual network name"
            placeholder="Search virtual networks by name"
            value={searchValue}
            onChange={handleSetSearchValue}
          />
          <Anchor component={Link} to='/virtualNetwork/new'>Create new network</Anchor>
        </Box>
        <DataTable
          minHeight={400}
          noRecordsText="No results for your search"
          flex={1}
          borderRadius="sm"
          withColumnBorders
          withTableBorder={false}
          striped
          highlightOnHover
          records={data?.data || []}
          columns={[
            { accessor: 'name', title: 'Virtual network' },
            { accessor: 'id', title: 'Id' },
          ]}
          onRowClick={({ record }) => attachNetwork(record.id)}
          />
      </Stack>
    </Drawer>
  )
}
