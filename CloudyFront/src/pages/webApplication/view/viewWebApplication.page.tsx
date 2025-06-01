import React from "react";
import { IconTrash } from '@tabler/icons-react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { Anchor, Divider, Stack, useMantineTheme } from '@mantine/core';
import { ResourceViewAuditLog } from '@/components/ResourceView/ResourceViewAuditLog';
import { ResourceViewLayout, ResourceViewPage } from '@/components/ResourceView/ResourceViewLayout';
import { ResourceViewSummary } from '@/components/ResourceView/ResourceViewSummary';
import { ResourceViewToolbar, ResourceViewToolbarItem } from '@/components/ResourceView/ResourceViewToolbar';
import { CloudyIconWebApplication, defaultIconStyle } from '@/icons/Resources';
import {
  useGetApiResourceWebApplicationResourceByIdQuery,
} from '@/services/rtk/cloudyApi';
import { viewResourceOfType } from '@/util/navigation';
import { WebApplicationDeploymentSubpage } from '@/sections/webApplication/view/WebApplicationDeploymentSubpage';
import { WebApplicationEnvironmentSubpage } from '@/sections/webApplication/view/WebApplicationEnvironmentSubpage';
import { WebApplicationNetworkSubpage } from '@/sections/webApplication/view/WebApplicationNetworkSubpage';


export function ViewWebApplicationPage() {
  const { id: resourceId } = useParams<{ id: string }>();
  const theme = useMantineTheme();
  const { data: resourceBaseData } = useGetApiResourceWebApplicationResourceByIdQuery({
    id: resourceId || '',
  });
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
            { name: 'Web application ID', value: { text: resourceBaseData?.id } },
            { name: 'Web application name', value: { text: resourceBaseData?.name } },
            { name: 'Networking', value: { text: 'Go to configuration', link: `${viewResourceOfType('WebApplicationResource', resourceBaseData?.id)}?rpi=4` } },
            { name: 'Created at', value: { text: new Date(resourceBaseData?.createdAt).toLocaleString() } },
          ]} />
          <Divider />
          TODO
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
        TODO
      </ResourceViewPage>
    </ResourceViewLayout>
  );
}
