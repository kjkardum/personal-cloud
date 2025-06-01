import React from 'react';
import { IconTrash } from '@tabler/icons-react';
import { DataTable } from 'mantine-datatable';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { Anchor, Divider, Stack, Title, useMantineTheme } from '@mantine/core';
import { ResourceViewAuditLog } from '@/components/ResourceView/ResourceViewAuditLog';
import { ResourceViewLayout, ResourceViewPage } from '@/components/ResourceView/ResourceViewLayout';
import { ResourceViewSummary } from '@/components/ResourceView/ResourceViewSummary';
import { ResourceViewToolbar, ResourceViewToolbarItem } from '@/components/ResourceView/ResourceViewToolbar';
import { CloudyIconVirtualNetworkResource, defaultIconStyle } from '@/icons/Resources';
import { useGetApiResourceVirtualNetworkResourceByIdQuery } from '@/services/rtk/cloudyApi';
import { viewResourceOfType } from '@/util/navigation';
import { TypeToIcon, TypeToText } from '@/util/typeToDisplay';


export function ViewVirtualNetworkPage() {
  const { id: resourceId } = useParams<{ id: string }>();
  const theme = useMantineTheme();
  const navigate = useNavigate();
  const { data: resourceBaseData } = useGetApiResourceVirtualNetworkResourceByIdQuery({
    id: resourceId || '',
  });
  return (
    <ResourceViewLayout
      title={
        <>
          <CloudyIconVirtualNetworkResource style={{ ...defaultIconStyle, marginRight: '4px' }} />
          {resourceBaseData?.name || 'Loading'}
        </>
      }
      subtitle={<>in <Anchor component={Link} to={viewResourceOfType('ResourceGroup', resourceBaseData?.resourceGroupId || '')}>{resourceBaseData?.resourceGroupName}</Anchor></>}
    >
      <ResourceViewPage title="Overview">
        <ResourceViewToolbar>
          <ResourceViewToolbarItem
            label="Delete TODO"
            leftSection={<IconTrash color={theme.colors[theme.primaryColor][4]} height={16} />}
            onClick={()=> alert('TODO')}
          />
        </ResourceViewToolbar>
        <Stack p='sm'>
          <ResourceViewSummary items={[
            { name: 'Resource group', value: { text: resourceBaseData?.resourceGroupName } },
            { name: 'Virtual network ID', value: { text: resourceBaseData?.id } },
            { name: 'Virtual network name', value: { text: resourceBaseData?.name } },
            { name: 'Created at', value: { text: new Date(resourceBaseData?.createdAt).toLocaleString() } },
          ]} />
          <Divider />
          <Title order={3}>Resource list</Title>
          <DataTable
            borderRadius="sm"
            withColumnBorders
            highlightOnHover
            records={resourceBaseData?.resources || []}
            columns={[
              { accessor: 'name', title: 'Name', render: ({name, resourceType}) => <>{TypeToIcon[resourceType]} {name}</> },
              { accessor: 'resourceType', title: 'Resource type', render: ({resourceType}) => TypeToText[resourceType] },
              { accessor: 'id', title: 'Resource ID' },
            ]}
            onRowClick={({ record }) => navigate(viewResourceOfType(record.resourceType, record.id))}
          />
        </Stack>
      </ResourceViewPage>
      <ResourceViewPage title="Audit log">
        <ResourceViewAuditLog resourceBaseData={resourceBaseData} />
      </ResourceViewPage>
    </ResourceViewLayout>
  );
}
