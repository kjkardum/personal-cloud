import React, { useMemo, useState } from 'react';
import { DataTable, DataTableColumn } from 'mantine-datatable';
import { AuditLogDto, useGetApiResourceBaseResourceByResourceIdAuditLogQuery } from '@/services/rtk/cloudyApi';
import { EmptyGuid } from '@/util/guid';
import { Stack, TextInput } from '@mantine/core';
import { useDebouncedValue } from '@mantine/hooks';

export const ResourceViewAuditLog = ({
  resourceBaseData,
}: {
  resourceBaseData: { id: string } | undefined;
}) => {
  const [auditPage, setAuditPage] = useState(1);
  const [searchQuery, setSearchQuery] = useState('');
  const [debouncedSearchQuery] = useDebouncedValue(searchQuery, 300);
  const { data: resourceAuditLogPaginated } =
    useGetApiResourceBaseResourceByResourceIdAuditLogQuery({
      resourceId: resourceBaseData?.id || '',
      page: auditPage,
      filterBy: debouncedSearchQuery,
    });
  const columns = useMemo(() => {
    const base: DataTableColumn<AuditLogDto>[] = [
      { accessor: 'actionDisplayText', title: 'Action' },
      {
        accessor: 'timestamp',
        title: 'Timestamp',
        render: ({ timestamp: date }) =>
          date && new Date(date.endsWith('Z') ? date : `${date}Z`).toLocaleString(),
      },
    ];
    if (resourceBaseData?.id === EmptyGuid) {
      base.push({ accessor: 'resourceId', title: 'Resource ID' });
    }
    return base;
  }, [resourceBaseData]);
  return (
    <Stack gap='md'>
      <TextInput
        m='sm'
        placeholder="Search audit log"
        value={searchQuery}
        onChange={(e) => setSearchQuery(e.currentTarget.value)}
        />
    <DataTable
      borderRadius="sm"
      withColumnBorders
      withTableBorder={false}
      striped
      highlightOnHover
      records={resourceAuditLogPaginated?.data || []}
      totalRecords={resourceAuditLogPaginated?.totalCount || 0}
      recordsPerPage={resourceAuditLogPaginated?.pageSize || 0}
      page={auditPage}
      onPageChange={(page) => setAuditPage(page)}
      columns={columns}
      onRowClick={({ record }) => alert(JSON.stringify(record))}
    />
    </Stack>
  );
};
