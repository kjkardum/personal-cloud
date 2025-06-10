import React, { useState } from 'react';
import { IconExternalLink } from '@tabler/icons-react';
import { DataTable } from 'mantine-datatable';
import { useNavigate } from 'react-router-dom';
import { Anchor, Group, Stack, Text, TextInput } from '@mantine/core';
import { useDebouncedValue } from '@mantine/hooks';
import { useGetApiResourceWebApplicationResourceQuery } from '@/services/rtk/cloudyApi';
import { viewResourceOfType } from '@/util/navigation';
import { TypeToIcon, TypeToText } from '@/util/typeToDisplay';

export const BrowseWebApplicationResourcePage = () => {
  const navigate = useNavigate();
  const [page, setPage] = useState(1);
  const [searchQuery, setSearchQuery] = useState('');
  const [debouncedSearchQuery] = useDebouncedValue(searchQuery, 300);
  const { data: paginationResult } = useGetApiResourceWebApplicationResourceQuery({
    page,
    filterBy: debouncedSearchQuery,
  });
  return (
    <Stack gap="md">
      <TextInput
        m="sm"
        placeholder="Search resources"
        value={searchQuery}
        onChange={(e) => setSearchQuery(e.currentTarget.value)}
      />
      <DataTable
        borderRadius="sm"
        withColumnBorders
        withTableBorder={false}
        striped
        highlightOnHover
        records={paginationResult?.data || []}
        totalRecords={paginationResult?.totalCount || 0}
        recordsPerPage={paginationResult?.pageSize || 0}
        page={page}
        onPageChange={(page) => setPage(page)}
        columns={[
          {
            accessor: 'name',
            title: 'Name',
            render: ({ name }) => (
              <>
                {TypeToIcon.WebApplicationResource} {name}
              </>
            ),
          },
          {
            accessor: 'publicProxyConfigurations',
            title: 'Public domains',
            render: ({ publicProxyConfigurations }) => {
              if (!publicProxyConfigurations?.length) {
                return 'No public domains';
              }
              return (
                <Group>
                  <Anchor
                    href={
                      (publicProxyConfigurations[0].useHttps ? 'https://' : 'http://') +
                      publicProxyConfigurations[0].domain
                    }
                    onClick={(e) => e.stopPropagation()}
                    target="_blank"
                  >
                    <Group gap={4}>
                      {publicProxyConfigurations[0].domain}
                      <IconExternalLink size={12} />
                    </Group>
                  </Anchor>
                  {publicProxyConfigurations.length > 1 && (
                    <Text>+ {publicProxyConfigurations.length - 1}</Text>
                  )}
                </Group>
              );
            },
          },
          { accessor: 'id', title: 'Resource ID' },
          {
            accessor: 'createdAt',
            title: 'Created At',
            render: ({ createdAt }) => createdAt && new Date(createdAt).toLocaleString(),
          },
        ]}
        onRowClick={({ record }) =>
          navigate(viewResourceOfType('WebApplicationResource', record.id))
        }
      />
    </Stack>
  );
};
