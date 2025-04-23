import React from 'react';
import { Divider, Flex, Stack, Title } from '@mantine/core';
import { DashboardButton } from '@/components/DashboardButton/DashboardButton';
import { DashboardButtonNewResource } from '@/components/DashboardButton/NewResource/DashboardButtonNewResource';
import {CloudyIconDatabase} from '@/icons/Resources';
import {useGetApiResourceBaseResourceQuery} from "@/services/rtk/cloudyApi";
import {TypeToIcon} from "@/util/typeToIcon";

export function HomePage() {
  const {data: recentlyCreatedResources} = useGetApiResourceBaseResourceQuery({orderBy: 'createdAt,Desc'});
  return (
    <>
      <Stack gap="lg" w="100%">
        <Title order={1}>Homepage</Title>
        <Divider />
        <Stack>
          <Title order={2}>Resources</Title>
          <Flex direction="row" wrap="wrap" gap="md" justify="flex-start">
            <DashboardButtonNewResource />
            {recentlyCreatedResources?.data.map(resource => (
                <DashboardButton icon={TypeToIcon[resource.resourceType]}>
                  <span style={{ whiteSpace: 'pre-wrap' }}>{resource.name}</span>
                </DashboardButton>
            ))}
          </Flex>
        </Stack>
      </Stack>
    </>
  );
}
