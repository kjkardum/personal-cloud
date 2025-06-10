import React, { useState } from 'react';
import { IconExternalLink } from '@tabler/icons-react';
import { DataTable } from 'mantine-datatable';
import { Link, useNavigate } from 'react-router-dom';
import { Anchor, Group, Stack, Text, TextInput } from '@mantine/core';
import { useDebouncedValue } from '@mantine/hooks';
import { useGetApiResourcePostgresServerResourceDatabaseQuery } from '@/services/rtk/cloudyApi';
import { viewResourceOfType } from '@/util/navigation';
import { TypeToIcon, TypeToText } from '@/util/typeToDisplay';
import { CloudyIconDatabaseServer } from '@/icons/Resources';

export const BrowsePostgresDatabaseResourcePage = () => {
  const navigate = useNavigate();
  const [page, setPage] = useState(1);
  const [searchQuery, setSearchQuery] = useState('');
  const [debouncedSearchQuery] = useDebouncedValue(searchQuery, 300);
  const { data: paginationResult } = useGetApiResourcePostgresServerResourceDatabaseQuery({
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
                {TypeToIcon.PostgresDatabaseResource} {name}
              </>
            ),
          },
          {
            accessor: 'server',
            title: 'Server',
            render: ({ serverName, serverId }) => (
              <Anchor component={Link} to={viewResourceOfType('PostgresServerResource', serverId)} onClick={(e) => e.stopPropagation()}>
                <Group gap={4}>
                  <CloudyIconDatabaseServer />
                  {serverName}
                </Group>
              </Anchor>
            ),
          },
          { accessor: 'id', title: 'Resource ID' },
          {
            accessor: 'createdAt',
            title: 'Created At',
            render: ({ createdAt }) => createdAt && new Date(createdAt).toLocaleString(),
          },
        ]}
        onRowClick={({ record }) =>
          navigate(viewResourceOfType('PostgresDatabaseResource', record.id))
        }
      />
    </Stack>
  );
};
