import React from 'react';
import { IconBrandDocker } from '@tabler/icons-react';
import { useNavigate } from 'react-router-dom';
import { Divider, Flex, Stack, Title } from '@mantine/core';
import { DashboardButton } from '@/components/DashboardButton/DashboardButton';
import { DashboardButtonNewResource } from '@/components/DashboardButton/NewResource/DashboardButtonNewResource';
import { CloudyIconDocker } from '@/icons/Resources';
import { useGetApiResourceBaseResourceQuery } from "@/services/rtk/cloudyApi";
import { viewResourceOfType, viewVirtualResource } from '@/util/navigation';
import { TypeToIcon } from "@/util/typeToDisplay";


export function HomePage() {
  const {data: recentlyCreatedResources} = useGetApiResourceBaseResourceQuery({orderBy: 'createdAt,Desc'});
  const navigate = useNavigate();
  return (
    <>
      <Stack gap="lg" w="100%" p="md">
        <Title order={1}>Homepage</Title>
        <Divider />
        <Stack>
          <Title order={2}>Resources</Title>
          <Flex direction="row" wrap="wrap" gap="md" justify="flex-start">
            <DashboardButtonNewResource />
            <DashboardButton icon={<CloudyIconDocker/>} onClick={()=>navigate(viewVirtualResource('CloudyDocker'))}>
              <span style={{ whiteSpace: 'pre-wrap' }}>Cloudy host</span>
            </DashboardButton>
            {recentlyCreatedResources?.data.map(resource => (
                <DashboardButton icon={TypeToIcon[resource.resourceType]} onClick={()=>navigate(viewResourceOfType(resource.resourceType, resource.id))} key={resource.id}>
                  <span style={{ whiteSpace: 'pre-wrap' }}>{resource.name}</span>
                </DashboardButton>
            ))}

          </Flex>
        </Stack>
      </Stack>
    </>
  );
}
