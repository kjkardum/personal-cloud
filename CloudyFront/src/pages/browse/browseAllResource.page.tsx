import React, { useState } from 'react';
import { useDebouncedValue } from '@mantine/hooks';
import { useGetApiResourceBaseResourceQuery } from '@/services/rtk/cloudyApi';
import { Stack, TextInput } from '@mantine/core';
import { DataTable } from 'mantine-datatable';
import { viewResourceOfType } from '@/util/navigation';
import { TypeToIcon, TypeToText } from '@/util/typeToDisplay';
import { useNavigate } from 'react-router-dom';

export const BrowseAllResourcePage = ({resourceType}: {resourceType: string | undefined}) => {
  const navigate = useNavigate();
  const [page, setPage] = useState(1);
  const [searchQuery, setSearchQuery] = useState('');
  const [debouncedSearchQuery] = useDebouncedValue(searchQuery, 300);
  const { data: paginationResult } = useGetApiResourceBaseResourceQuery({
    resourceType,
    page,
    filterBy: debouncedSearchQuery,
  });
  return (
    <Stack gap='md'>
      <TextInput
        m='sm'
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
          { accessor: 'name', title: 'Name', render: ({name, resourceType}) => <>{TypeToIcon[resourceType]} {name}</> },
          { accessor: 'resourceType', title: 'Resource type', render: ({resourceType}) => TypeToText[resourceType] },
          { accessor: 'id', title: 'Resource ID' },
          {
            accessor: 'createdAt',
            title: 'Created At',
            render: ({ createdAt }) => createdAt && new Date(createdAt).toLocaleString(),
          },
        ]}
        onRowClick={({ record }) => navigate(viewResourceOfType(record.resourceType, record.id))}
      />
    </Stack>
  )
};
