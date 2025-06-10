import React, { useEffect } from 'react';
import {
  IconLayoutSidebarLeftCollapse,
  IconLayoutSidebarLeftExpand,
  IconLogout,
  IconMoon,
  IconSettings,
  IconSun,
  IconUser,
} from '@tabler/icons-react';
import { Link, Outlet, useNavigate } from 'react-router-dom';
import { ActionIcon, AppShell, Burger, Button, Flex, Group, Menu, Skeleton, Stack, useMantineColorScheme } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { SearchBox } from '@/components/SearchBox/SearchBox';
import { CloudyIconDocker } from '@/icons/Resources';
import { sidebarItems } from '@/util/sidebar';
import { AuthGuard } from '../guards/AuthGuard';
import { viewVirtualResource } from '@/util/navigation';
import {
  useGetApiAuthenticationAuthenticatedQuery,
  usePostApiAuthenticationLogoutMutation,
} from '@/services/rtk/cloudyApi';


export default function Layout() {
  const navigate = useNavigate();

  const [logout] = usePostApiAuthenticationLogoutMutation();
  const {refetch: refetchSelf } = useGetApiAuthenticationAuthenticatedQuery();
  const handleLogout = async () => {
    await logout().unwrap();
    refetchSelf();
  };

  const [opened, { toggle }] = useDisclosure();
  const [iconsOnly, { toggle: toggleIconsOnly }] = useDisclosure();
  const { setColorScheme, colorScheme } = useMantineColorScheme();
  useEffect(() => {
    if (colorScheme === 'auto') {
      setColorScheme('light');
    }
  }, [colorScheme, setColorScheme]);

  const headerHeight = 60;
  return (
    <AuthGuard>
      <AppShell
        header={{ height: headerHeight }}
        navbar={{
          width: iconsOnly ? 60 : 220,
          breakpoint: 'sm',
          collapsed: { mobile: !opened },
        }}
        padding="0"
      >
        <AppShell.Header>
          <Flex justify="space-between" align="center" h="100%" px="sm">
            <Group h="100%" px="md">
              <Burger opened={opened} onClick={toggle} hiddenFrom="sm" size="sm" />
              Cloudy
            </Group>
            <SearchBox />
            <Group gap="sm">
              <ActionIcon variant="subtle" size="md" c={colorScheme === 'dark' ? 'yellow' : 'teal'}>
                {colorScheme === 'dark' ? (
                  <IconSun onClick={() => setColorScheme('light')} />
                ) : (
                  <IconMoon onClick={() => setColorScheme('dark')} />
                )}
              </ActionIcon>
              <Menu shadow="md" width={200}>
                <Menu.Target>
                  <ActionIcon variant="subtle" size="md" c="orange">
                    <IconUser />
                  </ActionIcon>
                </Menu.Target>

                <Menu.Dropdown>
                  <Menu.Label>Application</Menu.Label>
                  <Menu.Item component={Link} to={viewVirtualResource('CloudyDocker')} leftSection={<CloudyIconDocker size={14} />}>Manage host</Menu.Item>
                  <Menu.Item leftSection={<IconSettings color="lightblue" size={14} />}>Settings (todo)</Menu.Item>

                  <Menu.Divider />

                  <Menu.Label>Account</Menu.Label>
                  <Menu.Item onClick={handleLogout} leftSection={<IconLogout color='red' size={14} />}>
                    Logout
                  </Menu.Item>
                </Menu.Dropdown>
              </Menu>
            </Group>
          </Flex>
        </AppShell.Header>
        <AppShell.Navbar p="md">
          <Flex direction="column" justify="space-between" align="center" h="100%">
            <Stack gap="sm" w="100%">
              {sidebarItems.map((item) =>
                iconsOnly ? (
                  <ActionIcon
                    variant="default"
                    key={item.name}
                    onClick={() => item.href && navigate(item.href)}
                  >
                    {item.icon}
                  </ActionIcon>
                ) : (
                  <Button
                    variant="default"
                    leftSection={item.icon}
                    key={item.name}
                    justify={'space-end'}
                    onClick={() => item.href && navigate(item.href)}
                  >
                    {item.name}
                  </Button>
                )
              )}
            </Stack>
            <ActionIcon onClick={toggleIconsOnly} variant="filled" size="md" mt="md">
              {iconsOnly ? <IconLayoutSidebarLeftExpand /> : <IconLayoutSidebarLeftCollapse />}
            </ActionIcon>
          </Flex>
        </AppShell.Navbar>
        <AppShell.Main style={{ height: `calc(100vh - ${headerHeight}px)` }}>
          <Outlet />
        </AppShell.Main>
      </AppShell>
    </AuthGuard>
  );
}
