import React from 'react';
import { IconArrowRight, IconPlus } from '@tabler/icons-react';
import { Menu, rem, useMantineTheme } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { DashboardButton } from '@/components/DashboardButton/DashboardButton';
import {
  CloudyIconBrowseResources,
  CloudyIconDatabase,
  CloudyIconResourceGroup,
  CloudyIconWebApplication,
} from '@/icons/Resources';
import { CreateNewDatabaseDialog } from '@/sections/database/createNew/createNewDatabaseDialog';

export function DashboardButtonNewResource() {
  const [databaseDialogOpened, { open: openDatabaseDialog, close: closeDatabaseDialog }] = useDisclosure(false);
  return (
    <>
      <CreateNewDatabaseDialog open={databaseDialogOpened} onClose={closeDatabaseDialog} />
      <Menu
        transitionProps={{ transition: 'pop-top-left' }}
        position="bottom-start"
        width={220}
        withinPortal
      >
        <Menu.Target>
          <DashboardButton
            icon={<IconPlus style={{ width: rem(18), height: rem(18) }} stroke={1.5} />}
          >
            New Resource
          </DashboardButton>
        </Menu.Target>
        <Menu.Dropdown>
          <Menu.Item leftSection={<CloudyIconWebApplication />}>Web application</Menu.Item>
          <Menu.Item leftSection={<CloudyIconDatabase />} onClick={openDatabaseDialog}>Database</Menu.Item>
          <Menu.Item leftSection={<CloudyIconResourceGroup />}>Resource group</Menu.Item>
          <Menu.Item
            leftSection={<CloudyIconBrowseResources />}
            rightSection={<IconArrowRight style={{ width: rem(16), height: rem(16) }} />}
          >
            Something else
          </Menu.Item>
        </Menu.Dropdown>
      </Menu>
    </>
  );
}
