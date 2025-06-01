import React from 'react';
import { IconArrowRight, IconPlus } from '@tabler/icons-react';
import { Menu, rem, useMantineTheme } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { DashboardButton } from '@/components/DashboardButton/DashboardButton';
import {
  CloudyIconBrowseResources,
  CloudyIconDatabase, CloudyIconKafkaResource,
  CloudyIconResourceGroup,
  CloudyIconWebApplication,
} from '@/icons/Resources';
import { CreateNewDatabaseDialog } from '@/sections/database/general/createNewDatabaseDialog';
import { useNavigate } from 'react-router-dom';

export function DashboardButtonNewResource() {
  const [databaseDialogOpened, { open: openDatabaseDialog, close: closeDatabaseDialog }] = useDisclosure(false);
  const navigate = useNavigate();
  const openMessageBrokerDialog = () => {
    //TODO: Implement message broker selector dialog
    alert('Message broker selector is not implemented yet. Navigating to kafka creation');
    navigate('/kafka/new/cluster');
  }
  const goToNewWebApplicationPage = () => {
    navigate('webApplication/new')
  }
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
          <Menu.Item leftSection={<CloudyIconWebApplication />} onClick={goToNewWebApplicationPage}>Web application</Menu.Item>
          <Menu.Item leftSection={<CloudyIconDatabase />} onClick={openDatabaseDialog}>Database</Menu.Item>
          <Menu.Item leftSection={<CloudyIconKafkaResource />} onClick={openMessageBrokerDialog}>Message broker</Menu.Item>
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
