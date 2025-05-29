import React, { useEffect } from 'react';
import { DataTable } from 'mantine-datatable';
import { useDispatch } from 'react-redux';
import { Anchor, Stack, Title } from '@mantine/core';
import { KafkaTopicDto, useGetApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdConsumeLiveQuery, useGetApiResourceKafkaClusterResourceByServerIdTopicsQuery, useLazyGetApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdConsumeLiveQuery } from '@/services/rtk/cloudyApi';
import { useStreamApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdConsumeLiveQuery } from '@/services/rtk/enhancements';
import { startTopicStream, stopTopicStream } from '@/store/stores/ApiHelperStore';


export const KafkaTopicsSubpage = ({ resourceId }: { resourceId: string }) => {
  const { data: kafkaTopics } = useGetApiResourceKafkaClusterResourceByServerIdTopicsQuery({
    serverId: resourceId,
  });
  const [selectedTopic, setSelectedTopic] = React.useState<KafkaTopicDto | null>(null);
  const dispatch = useDispatch();
  const {data: consumeData } = useStreamApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdConsumeLiveQuery({
    serverId: resourceId,
    topicId: selectedTopic?.name || '',
  });

  const handleClickTopic = (topic: KafkaTopicDto) => {
    if (selectedTopic?.name) {
      dispatch(stopTopicStream({serverId: resourceId, topic: selectedTopic.name}));
    }
    if (topic?.name) {
      dispatch(startTopicStream({ serverId: resourceId, topic: topic.name }));
    }
    setSelectedTopic(topic);
  }
  useEffect(() => () => {
      console.log(`Stopping stream for topic: ${selectedTopic?.name}`);
      if (selectedTopic?.name) {
        dispatch(stopTopicStream({ serverId: resourceId, topic: selectedTopic.name }));
      }
    }, [selectedTopic]
  );

  return (
    <Stack spacing="md">
      {selectedTopic && (
        <>
          <Anchor component="button" onClick={() => handleClickTopic(null)} ta="left">
            Back to topics
          </Anchor>
          <Title order={3}>
            Topic: {selectedTopic.name} (ID: {selectedTopic.topicId})
          </Title>
          CONSUME:
          <pre>{consumeData.join("\n")}</pre>
        </>
      )}
      <DataTable
        borderRadius="sm"
        withColumnBorders
        highlightOnHover
        records={selectedTopic?.partitions || kafkaTopics || []}
        columns={
          selectedTopic
            ? [
                { accessor: 'partition', title: 'Partition' },
                { accessor: 'leader', title: 'Leader' },
                {
                  accessor: 'replicas',
                  title: 'Replicas',
                  render: ({ replicas }) => replicas.join(', '),
                },
                { accessor: 'isr', title: 'ISR', render: ({ isr }) => isr.join(', ') },
                { accessor: 'elr', title: 'ELR' },
                { accessor: 'lastKnownElr', title: 'Last Known ELR' },
              ]
            : [
                { accessor: 'name', title: 'Name' },
                { accessor: 'topicId', title: 'Topic ID' },
                { accessor: 'partitionCount', title: 'Partition count' },
                { accessor: 'replicationFactor', title: 'Replication factor' },
              ]
        }
        onRowClick={({ record }) => !selectedTopic && handleClickTopic(record as KafkaTopicDto)}
      />
    </Stack>
  );
};
