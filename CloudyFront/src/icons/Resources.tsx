import React from 'react';
import {
  IconBrandDocker,
  IconBrowser, IconCloudNetwork,
  IconDatabase,
  IconDatabaseShare,
  IconFolder,
  IconHome, IconMessage,
  IconPackage,
  IconProps, IconTopologyRing2,
} from '@tabler/icons-react';
import {rem, useMantineColorScheme, useMantineTheme} from '@mantine/core';

export const defaultIconStyle = { width: rem(16), height: rem(16) };

const defaultIconProps = {
  style: defaultIconStyle,
  stroke: 1.5,
};
export const CloudyIconDocker = (props: IconProps) => {
  const theme = useMantineTheme();
  return <IconBrandDocker {...defaultIconProps} color={theme.colors.orange[6]} {...props} />;
}

export const CloudyIconWebApplication = (props: IconProps) => {
  const theme = useMantineTheme();
  return <IconBrowser {...defaultIconProps} color={theme.colors.blue[6]} {...props} />;
};

export const CloudyIconDatabase = (props: IconProps) => {
  const theme = useMantineTheme();
  return <IconDatabaseShare {...defaultIconProps} color={theme.colors.pink[6]} {...props} />;
};

export const CloudyIconDatabaseServer = (props: IconProps) => {
  const theme = useMantineTheme();
  return <IconDatabase {...defaultIconProps} color={theme.colors.pink[6]} {...props} />;
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

export const CloudyIconKafkaResource = (props: IconProps) => {
  const  theme = useMantineTheme();
  return <IconMessage {...defaultIconProps} color={theme.colors.blue[6]} {...props} />;
}

export const CloudyIconVirtualNetworkResource = (props: IconProps) => {
  const  theme = useMantineTheme();
  return <IconTopologyRing2 {...defaultIconProps} color={theme.colors.green[6]} {...props} />;
}

export const CloudyIconNetwork = (props: IconProps) => {
  const  theme = useMantineTheme();
  return <IconCloudNetwork {...defaultIconProps} color={theme.colors.blue[6]} {...props} />;

}
