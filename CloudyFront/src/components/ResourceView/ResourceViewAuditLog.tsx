import {
  useGetApiResourceBaseResourceByResourceIdAuditLogQuery,
} from '@/services/rtk/cloudyApi';
import React, { useState } from 'react';
import { DataTable } from 'mantine-datatable';

export const ResourceViewAuditLog =  ({resourceBaseData}: { resourceBaseData: {id: string} | undefined }) => {
  const [auditPage, setAuditPage] = useState(1);
  const { data: resourceAuditLogPaginated } = useGetApiResourceBaseResourceByResourceIdAuditLogQuery({
    resourceId: resourceBaseData?.id || '',
    page: auditPage,
  });
  return (
    <DataTable
      borderRadius="sm"
      withColumnBorders
      striped
      highlightOnHover
      records={resourceAuditLogPaginated?.data || []}
      totalRecords={resourceAuditLogPaginated?.totalCount || 0}
      recordsPerPage={resourceAuditLogPaginated?.pageSize || 0}
      page={auditPage}
      onPageChange={(page) => setAuditPage(page)}
      columns={[
        { accessor: 'actionDisplayText', title: 'Action' },
        { accessor: 'timestamp', title: 'Timestamp', render: ({timestamp: date}) => new Date(date.endsWith('Z') ? date : date + 'Z').toLocaleString() },
      ]}
      onRowClick={({ record }) => alert(JSON.stringify(record))}
    />
  )
}
