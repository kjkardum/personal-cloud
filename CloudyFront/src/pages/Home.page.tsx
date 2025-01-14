import React from 'react';
import { Divider, Flex, Stack, Title } from '@mantine/core';
import { DashboardButton } from '@/components/DashboardButton/DashboardButton';
import { DashboardButtonNewResource } from '@/components/DashboardButton/NewResource/DashboardButtonNewResource';
import { CloudyIconDatabase } from '@/icons/Resources';

export function HomePage() {
  return (
    <>
      <Stack gap="lg" w="100%">
        <Title order={1}>Homepage</Title>
        <Divider />
        <Stack>
          <Title order={2}>Resources</Title>
          <Flex direction="row" wrap="wrap" gap="md" justify="flex-start">
            <DashboardButtonNewResource />
            <DashboardButton icon={<CloudyIconDatabase />}>
              <span style={{ whiteSpace: 'pre-wrap' }}>AzWeSQLDbLightspeedPart</span>
            </DashboardButton>
            <DashboardButton icon={<CloudyIconDatabase />}>OtherDb</DashboardButton>
          </Flex>
        </Stack>
      </Stack>
    </>
  );
}
