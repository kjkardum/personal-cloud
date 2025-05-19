import { Box, Divider, Flex, NavLink, NavLinkProps } from '@mantine/core';


export const ResourceViewToolbarItem = (props: NavLinkProps) => (
  <NavLink
    styles={{root: {width: "unset"}, ...props.styles}}
    {...props}
  />
)

type ResourceViewToolbarItem = React.ReactElement<typeof ResourceViewToolbarItem>;

export const ResourceViewToolbar = ({children}:{
  children: ResourceViewToolbarItem[] | ResourceViewToolbarItem,
}) => {
  return (
    <Box w='100%'>
      <Flex>
        {children}
      </Flex>
      <Divider/>
    </Box>
  );
}


