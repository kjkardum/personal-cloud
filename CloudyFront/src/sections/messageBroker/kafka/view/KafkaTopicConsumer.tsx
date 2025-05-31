import React, { memo, useCallback, useEffect, useMemo, useState } from 'react';
import { DataTable } from 'mantine-datatable';
import { useDispatch } from 'react-redux';
import {
  Box,
  Code,
  Title,
  useMantineColorScheme,
  useMantineTheme,
} from '@mantine/core';
import { cloudyApi } from '@/services/rtk/cloudyApi';
import {
  apiWithMoreEndpoints,
  useStreamApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdConsumeLiveQuery,
} from '@/services/rtk/enhancements';
import { startTopicStream, stopTopicStream } from '@/store/stores/ApiHelperStore';


export const KafkaTopicConsumer = ({resourceId, topicName}: {
    resourceId: string;
    topicName: string;
    }) => {
  const { colorScheme } = useMantineColorScheme();
  const theme = useMantineTheme();
  const [selectedTopic, setSelectedTopic] = useState<string | null>(null);
  const dispatch = useDispatch();
  const {data: consumeData } = useStreamApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdConsumeLiveQuery({
    serverId: resourceId,
    topicId: selectedTopic || '',
  });
  const [selectedMessage, setSelectedMessage] = useState<string | null>(null);
  const formattedData = useMemo(()=> (consumeData ?? [])
    .toReversed()
    .map(m=>m.slice(m.indexOf("Key="), m.indexOf("--=--"))).map(m => ({
      key: m.slice(m.indexOf("Key=") + 4, m.indexOf("\n")).trim(),
      partition: m.slice(m.indexOf("Partition=") + 10, m.indexOf("\n", m.indexOf("Partition="))).trim(),
      offset: m.slice(m.indexOf("Offset=") + 7, m.indexOf("\n", m.indexOf("Offset="))).trim(),
      message: m.slice(m.indexOf("Value=") + 6).trim(),
    })), [consumeData]);

  useEffect(() => {
    if (topicName) {
      dispatch(startTopicStream({ serverId: resourceId, topic: topicName }));
      setSelectedTopic(topicName);
    }
    return () => {
      if (selectedTopic) {
        console.log(`!!! !!! Stopping stream for topic: ${selectedTopic}`);
        dispatch(stopTopicStream({ serverId: resourceId, topic: selectedTopic }));
        dispatch(apiWithMoreEndpoints.util.invalidateTags(['KafkaClusterResourceStreams']));
      }
    };
  }, [resourceId, topicName, selectedTopic]);

  const tryParseMessage = useCallback((message: string) => {
    try {
      return JSON.stringify(JSON.parse(message), null, 2);
    } catch (e) {
      return message;
    }
  }, []);

  return (
        <Box pos='relative' h='400px' w='100%' style={{ overflowY: 'auto' }}>
          <DataTable
            borderRadius="sm"
            withColumnBorders
            withTableBorder={false}
            highlightOnHover
            records={formattedData}
            columns={[
                { accessor: 'key', title: 'Key' },
                { accessor: 'partition', title: 'Partition' },
                { accessor: 'offset', title: 'Offset' },
                { accessor: 'message', title: 'Message', render: ({ message }) => (message.length > 100 ? `${message.slice(0, 100)}...` : message) },
              ]}
            onRowClick={({ record }) => setSelectedMessage(record.message)}
          />
            {selectedMessage && (
                <Box pos='absolute' px='md' top={0} right={0} h='100%' w='300px' style={{ overflow: 'auto', zIndex: 3, border: '1px solid lightgray' }} bg={colorScheme === 'dark' ? 'black' : 'white'}>
                    <Title order={4} mb='md'>Selected Message</Title>
                    <Code style={{whiteSpace: 'pre-wrap'}}>{tryParseMessage(selectedMessage)}</Code>
                </Box>
            )}
        </Box>
    );
}
