import { Box, Divider, Flex, NavLink, Text, Title } from '@mantine/core';
import React, { Children, useEffect } from 'react';
import { useParams, useSearchParams } from 'react-router-dom';

export const ResourceViewPage = ({children}: {
  children: React.ReactNode[] | React.ReactNode,
  title: string,
  icon?: React.ReactNode,
}) => <> {children} </>;

type ResourceViewPage = React.ReactElement<typeof ResourceViewPage>;

export const ResourceViewLayout  = ({
  title,
  subtitle,
  children
}: {children: ResourceViewPage[] | ResourceViewPage, title: React.ReactNode, subtitle?: React.ReactNode}) => {
  const [searchParams] = useSearchParams();
  const [activeChild, setActiveChild] = React.useState(0);
  useEffect(() => {
    const asNumber = Number(searchParams.get('rpi'));
    if (asNumber) {
      setActiveChild(asNumber);
    }
  }, [searchParams]);
  return (
    <div style={{height:'100%'}}>
        <Flex direction="column" w="100%" h="100%">
          <Box p={4}>
            <Title order={1}>{title}</Title>
            {subtitle && <Text c="darkgray">{subtitle}</Text>}
          </Box>
          <Divider />
          <Flex direction="row" mih={0} flex={1} w="100%">
            <Box w={230}>
              {Children.map(children, (child, index) => (
                <NavLink
                  // @ts-expect-error props not recognized
                  key={child.props.title}
                  // @ts-expect-error props not recognized
                  label={child.props.title}
                  // @ts-expect-error props not recognized
                  leftSection={child.props.icon}
                  active={index === activeChild}
                  onClick={() => setActiveChild(index)}
                />
              ))}
            </Box>
            <Divider orientation='vertical' />
            <Box flex={1} miw={0} style={{overflowX: 'hidden'}}>
              {Array.isArray(children) && children.length > 1 ? children[activeChild] : children}
            </Box>
          </Flex>
        </Flex>
    </div>
  );
};
