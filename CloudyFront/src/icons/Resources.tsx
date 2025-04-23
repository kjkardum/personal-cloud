import React from 'react';
import {IconBrowser, IconDatabase, IconFolder, IconHome, IconPackage, IconProps, IconServer} from '@tabler/icons-react';
import {rem, useMantineColorScheme, useMantineTheme} from '@mantine/core';

const defaultIconProps = {
  style: { width: rem(16), height: rem(16) },
  stroke: 1.5,
};

export const CloudyIconWebApplication = (props: IconProps) => {
  const theme = useMantineTheme();
  return <IconBrowser {...defaultIconProps} color={theme.colors.blue[6]} {...props} />;
};

export const CloudyIconDatabase = (props: IconProps) => {
  const theme = useMantineTheme();
  return <IconDatabase {...defaultIconProps} color={theme.colors.pink[6]} {...props} />;
};

export const CloudyIconDatabaseServer = (props: IconProps) => {
  const theme = useMantineTheme();
  return < ><IconDatabase {...defaultIconProps} color={theme.colors.pink[6]} {...props} /> <IconServer {...defaultIconProps} color={theme.colors.pink[6]} {...props} /></>;
};

export const CloudyIconResourceGroup = (props: IconProps) => {
  const theme = useMantineTheme();
  return <IconFolder {...defaultIconProps} color={theme.colors.cyan[6]} {...props} />;
};

export const CloudyIconBrowseResources = (props: IconProps) => {
  const theme = useMantineTheme();
  return <IconPackage {...defaultIconProps} color={theme.colors.violet[6]} {...props} />;
};

export const CloudyIconHomepage = (props: IconProps) => {
  const { colorScheme } = useMantineColorScheme();
  return <IconHome {...defaultIconProps} color={colorScheme === 'dark' ? 'yellow' : 'teal'} {...props} />;
}
