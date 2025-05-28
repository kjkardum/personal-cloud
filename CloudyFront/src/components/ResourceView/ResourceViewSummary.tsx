import React from 'react';
import { Link } from 'react-router-dom';
import { Box, NavLink, Table, Title, useMantineTheme } from '@mantine/core';


export const ResourceViewSummary = ({items}: {
  items: {
    name: string,
    value: { text: string | undefined, link?: string, replace?: boolean }
  }[]
}) => {
  const theme = useMantineTheme();
  return (
  <Box>
    <Title order={3}>General information</Title>
    <Table withRowBorders={false}>
      <tbody>
        {items.map((item) => (
          <tr key={item.name}>
            <td>{item.name}</td>
            <td>
              {item.value.link ? (
                <NavLink component={Link} replace={item.value.replace ?? true} to={item.value.link} label={item.value.text ?? 'Loading...'} p={0} c={theme.colors[theme.primaryColor][4]}/>
              ) : (
                item.value.text ?? 'Loading...'
              )}
            </td>
          </tr>
        ))}
      </tbody>
    </Table>
  </Box>
  )
}
