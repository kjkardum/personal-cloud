import { useEffect, useState, useRef, useMemo } from 'react';
import {
  PredefinedLokiQuery,
  usePostApiResourceBaseResourceByResourceIdLokiMutation,
} from '@/services/rtk/cloudyApi';
import { DataTable } from 'mantine-datatable';
import { Text, Loader, Tooltip, Group, Badge, Center, Title, Stack, Button, ActionIcon } from '@mantine/core';
import { IconPlayerPause, IconPlayerPlay, IconRefresh } from '@tabler/icons-react';
import { useInterval } from '@mantine/hooks';

type LogEntry = {
  id: string;
  timestamp: string;
  level: string;
  containerName: string;
  message: string;
};
const limitUsed = 50;

export const ResourceViewLogs = ({
                                   resourceBaseData,
                                 }: {
  resourceBaseData: { id: string };
}) => {
  const resourceId = resourceBaseData.id;
  const [getLogs] = usePostApiResourceBaseResourceByResourceIdLokiMutation();

  const [logEntries, setLogEntries] = useState<LogEntry[]>([]);
  const [loadingInitial, setLoadingInitial] = useState(true);
  const [loadingMore, setLoadingMore] = useState(false);
  const [refresh, setRefresh] = useState<{} | null>(null);
  const [autoRefresh, setAutoRefresh] = useState(false);
  const [hasMore, setHasMore] = useState(true);
  const entriesDatesRange = useMemo(() => {
    if (logEntries.length === 0) { return; }
    const start = new Date(logEntries[logEntries.length - 1].timestamp);
    const end = new Date(logEntries[0].timestamp);
    return { start, end };
  }, [logEntries, resourceId]);

  // Keep track of the current 'end' timestamp for fetching older logs
  const endTimestampRef = useRef<Date>(new Date());
  const scrollViewportRef = useRef<HTMLDivElement>(null);

  const loadLogs = async (start: Date, end: Date, append = false) => {
    try {
      const response = await getLogs({
        resourceId,
        queryLokiQuery: {
          query: PredefinedLokiQuery.ContainerLog,
          start: start.toISOString(),
          end: end.toISOString(),
          step: undefined,
          limit: limitUsed
        },
      }).unwrap();

      const flattenedLogs: LogEntry[] = [];

      response?.data?.result?.forEach((resultItem) => {
        const stream = resultItem.stream;
        const level = stream?.detected_level || 'unknown';
        const containerName = stream?.container_name || 'N/A';

        resultItem?.values?.forEach(([ts, msg]) => {
          const timestamp = new Date(Number(ts.slice(0, -6))).toISOString(); // Convert ns to ms
          flattenedLogs.push({
            id: ts,
            timestamp,
            level,
            containerName,
            message: msg,
          });
        });
      });

      // Sort descending
      flattenedLogs.sort(
        (a, b) =>
          new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
      );

      if (append) {
        // Append and deduplicate by ID
        setLogEntries((prev) => {
          const combined = [...prev, ...flattenedLogs];
          const unique = Array.from(
            new Map(combined.map((item) => [item.id, item])).values()
          );
          return unique.sort(
            (a, b) =>
              new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
          );
        });
      } else {
        setLogEntries(flattenedLogs);
      }

      // If less than limitUsed results returned, no more data
      if (flattenedLogs.length < limitUsed) {
        setHasMore(false);
      }

      if (flattenedLogs.length > 0) {
        const lastLog = flattenedLogs[flattenedLogs.length - 1];
        endTimestampRef.current = new Date(lastLog.timestamp);
      }
    } catch (error) {
      console.error('Failed to load logs:', error);
      setHasMore(false);
    }
  };
  const doRefresh = () => {
    setRefresh({});
    setLoadingInitial(true);
    setLogEntries([]);
    setHasMore(true);
    endTimestampRef.current = new Date();
  }
  useInterval(
    () => autoRefresh && doRefresh(),
    3500,
    { autoInvoke: true }
  );

  useEffect(() => {
    const end = new Date();
    const start = new Date(end.getTime() - 24 * 60 * 60 * 1000); // 24 hours ago
    loadLogs(start, end).finally(() => setLoadingInitial(false));
  }, [resourceId, refresh]);

  const loadMoreRecords = async () => {
    if (!hasMore || loadingMore) return;
    setLoadingMore(true);

    const newEnd = endTimestampRef.current;
    const newStart = new Date(newEnd.getTime() - 24 * 60 * 60 * 1000);
    await loadLogs(newStart, newEnd, true);
    setLoadingMore(false);
  };

  return (
    <Stack h='100%'>
      <Group justify="space-between" mb="md">
        <Title order={3}>Logs Viewer</Title>
        {loadingInitial ? <Loader size="sm" /> : <div>
          <ActionIcon variant="subtle" size="md" mt='md' mr='md' onClick={doRefresh}><IconRefresh /></ActionIcon>
          <Tooltip label="Auto refresh">
            <ActionIcon variant="subtle" size="md" mt='md' mr='md' onClick={()=>setAutoRefresh(r => !r)}>
              {autoRefresh ? <IconPlayerPause /> : <IconPlayerPlay />}</ActionIcon>
          </Tooltip>
        </div>}
      </Group>
      <Text c="darkgray">Logs loaded from {entriesDatesRange?.start.toISOString()} to {entriesDatesRange?.end.toISOString()}</Text>
      <DataTable
        flex={1}
        highlightOnHover
        records={logEntries}
        columns={[
          {
            accessor: 'level',
            title: 'Level',
            render: (record) => (
              <Badge
                color={
                  record.level === 'error'
                    ? 'red'
                    : record.level === 'warn'
                      ? 'yellow'
                      : record.level === 'info'
                        ? 'green'
                        : 'gray'
                }
                variant="light"
              >
                {record.level.toUpperCase()}
              </Badge>
            ),
            width: '130px'
          },
          {
            accessor: 'message',
            title: 'Message',
            width: '60%',
            render: (record) => (
              <Text size="sm" style={{ whiteSpace: 'pre-wrap' }}>
                {record.message}
              </Text>
            ),
          },
          {
            accessor: 'timestamp',
            title: 'Timestamp',
            render: (record) => new Date(record.timestamp).toLocaleString(),
          },
        ]}
        noRecordsText="No logs found"
        onScrollToBottom={loadMoreRecords}
        scrollViewportRef={scrollViewportRef}
      />
      {loadingMore && (
        <Center mt="md">
          <Loader size="sm" />
        </Center>
      )}
    </Stack>
  );
};
