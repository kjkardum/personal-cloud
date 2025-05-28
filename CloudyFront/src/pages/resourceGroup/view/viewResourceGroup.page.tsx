import React from "react";
import { IconTrash } from '@tabler/icons-react';
import { DataTable } from 'mantine-datatable';
import { useNavigate, useParams } from 'react-router-dom';
import { Divider, Stack, Title, useMantineTheme } from '@mantine/core';
import { ResourceViewAuditLog } from '@/components/ResourceView/ResourceViewAuditLog';
import { ResourceViewLayout, ResourceViewPage } from '@/components/ResourceView/ResourceViewLayout';
import { ResourceViewSummary } from '@/components/ResourceView/ResourceViewSummary';
import { ResourceViewToolbar, ResourceViewToolbarItem } from '@/components/ResourceView/ResourceViewToolbar';
import { CloudyIconDatabase, defaultIconStyle } from '@/icons/Resources';
import { useGetApiResourceResourceGroupByIdQuery } from '@/services/rtk/cloudyApi';
import { viewResourceOfType } from '@/util/navigation';
import { TypeToIcon, TypeToText } from '@/util/typeToDisplay';


export function ViewResourceGroupPage() {
  const { id: resourceId } = useParams<{ id: string }>();
  const theme = useMantineTheme();
  const navigate = useNavigate();
  const { data: resourceBaseData } = useGetApiResourceResourceGroupByIdQuery({
    id: resourceId || '',
  });
  return (
    <ResourceViewLayout
      title={
        <>
          <CloudyIconDatabase style={{ ...defaultIconStyle, marginRight: '4px' }} />
          {resourceBaseData?.name || 'Loading'}
        </>
      }
      subtitle="Resource group"
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
            { name: 'Resource group', value: { text: resourceBaseData?.name } },
            { name: 'Server ID', value: { text: resourceBaseData?.id } },
            { name: 'Resources', value: { text: `${resourceBaseData?.resources?.length ?? 0} resources created` } },
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
