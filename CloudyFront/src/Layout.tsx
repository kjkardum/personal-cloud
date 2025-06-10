import React, { useEffect } from 'react';
import {
  IconLayoutSidebarLeftCollapse,
  IconLayoutSidebarLeftExpand,
  IconMoon,
  IconSun,
} from '@tabler/icons-react';
import {
  ActionIcon,
  AppShell,
  Burger,
  Button,
  Flex,
  Group,
  Skeleton,
  Stack,
  useMantineColorScheme,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { SearchBox } from '@/components/SearchBox/SearchBox';
import { sidebarItems } from '@/util/sidebar';
import {Outlet, useNavigate} from "react-router-dom";

export default function Layout() {
  const [opened, { toggle }] = useDisclosure();
  const [iconsOnly, { toggle: toggleIconsOnly }] = useDisclosure();
  const navigate = useNavigate();
  const { setColorScheme, colorScheme } = useMantineColorScheme();
  useEffect(() => {
    if (colorScheme === 'auto') {
      setColorScheme('light');
    }
  }, [colorScheme, setColorScheme]);

  const headerHeight =  60;
  return (
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
          <ActionIcon variant="subtle" size="md" c={colorScheme === 'dark' ? 'yellow' : 'teal'}>
            {colorScheme === 'dark' ? (
              <IconSun onClick={() => setColorScheme('light')} />
            ) : (
              <IconMoon onClick={() => setColorScheme('dark')} />
            )}
          </ActionIcon>
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
      <AppShell.Main style={{height: `calc(100vh - ${headerHeight}px)`}}><Outlet /></AppShell.Main>
    </AppShell>
  );
}
