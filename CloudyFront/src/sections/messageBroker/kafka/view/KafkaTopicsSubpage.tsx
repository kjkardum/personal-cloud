import React, { memo, useCallback, useEffect } from 'react';
import { IconMessageUp, IconPlus } from '@tabler/icons-react';
import { DataTable } from 'mantine-datatable';
import { useDispatch } from 'react-redux';
import { z } from 'zod';
import {
  Anchor,
  Box,
  Button,
  Modal,
  NumberInput,
  Stack,
  Textarea,
  TextInput,
  Title,
  useMantineTheme,
} from '@mantine/core';
import { useForm, zodResolver } from '@mantine/form';
import { useDisclosure } from '@mantine/hooks';
import { ResourceViewToolbar, ResourceViewToolbarItem } from '@/components/ResourceView/ResourceViewToolbar';
import { KafkaTopicConsumer } from '@/sections/messageBroker/kafka/view/KafkaTopicConsumer';
import {
  CreateKafkaTopicCommand,
  CreatePostgresDatabaseCommand,
  KafkaTopicDto,
  ProduceKafkaTopicMessageCommand,
  useGetApiResourceKafkaClusterResourceByServerIdTopicsQuery,
  usePostApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdMutation,
  usePostApiResourceKafkaClusterResourceByServerIdTopicsMutation,
} from '@/services/rtk/cloudyApi';


export const KafkaTopicsSubpage = ({ resourceId }: { resourceId: string }) => {
  const theme = useMantineTheme();
  const { data: kafkaTopics, refetch: refetchKafkaTopics } = useGetApiResourceKafkaClusterResourceByServerIdTopicsQuery({
    serverId: resourceId,
  });
  const [selectedTopic, setSelectedTopic] = React.useState<KafkaTopicDto | null>(null);

  const [createTopicMutation, { isLoading: isCreatingTopic }] =
    usePostApiResourceKafkaClusterResourceByServerIdTopicsMutation();
  const [openedAddTopic, { open: openAddTopic, close: closeAddTopic }] = useDisclosure(false);
  const form = useForm<CreateKafkaTopicCommand>({
    mode: 'uncontrolled',
    initialValues: { topicName: '' },
    validate: zodResolver(
      z.object({
        topicName: z.string().nonempty('Topic name is required'),
      })
    ),
  });
  const submitNewTopicForm = async (values: CreateKafkaTopicCommand) => {
    await createTopicMutation({
      createKafkaTopicCommand: values,
      serverId: resourceId || '',
    }).unwrap();
    form.reset();
    refetchKafkaTopics();
    closeAddTopic();
  };

  const [createTopicMessageMutation, { isLoading: isCreatingMessage }] = usePostApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdMutation();
  const [openedAddMessage, { open: openAddMessage, close: closeAddMessage }] = useDisclosure(false);
  const producerForm = useForm<ProduceKafkaTopicMessageCommand>({
    mode: 'uncontrolled',
    initialValues: { key: '', value: '' },
    validate: zodResolver(
      z.object({
        value: z.string().nonempty('Message is required'),
      })
    ),
  });
  const submitProduceMessageForm = useCallback(async (values: ProduceKafkaTopicMessageCommand) => {
    await createTopicMessageMutation({
      produceKafkaTopicMessageCommand: values,
      topicId: selectedTopic?.name || '',
      serverId: resourceId || '',
    }).unwrap();
    producerForm.reset();
    closeAddMessage();
  }, [createTopicMessageMutation, producerForm, selectedTopic, resourceId, closeAddMessage]);
  return (
    <Stack gap="md">
      <Modal opened={openedAddTopic} onClose={closeAddTopic} title="Add topic manually to kafka">
        <form onSubmit={form.onSubmit(submitNewTopicForm)}>
          <TextInput
            label="Topic name"
            placeholder="Topic name"
            key={form.key('topicName')}
            {...form.getInputProps('topicName')}
          />
        <Button
          loading={isCreatingTopic}
          variant="outline"
          color="red"
          type="submit"
          style={{ marginTop: '16px' }}>Create</Button>
        <Button
          variant="default"
          type="button"
          onClick={closeAddTopic}
          style={{ marginTop: '16px', marginLeft: '8px' }}>Cancel</Button>
        </form>
      </Modal>
      <Modal opened={openedAddMessage} onClose={closeAddMessage} title="Add topic manually to kafka">
        <form onSubmit={producerForm.onSubmit(submitProduceMessageForm)}>
          <Textarea
            label="Message"
            placeholder="Message"
            key={producerForm.key('value')}
            {...producerForm.getInputProps('value')}
          />
          <TextInput
            label="Key (optional)"
            placeholder="Key (eg. 1) for partitioning"
            key={producerForm.key('key')}
            {...producerForm.getInputProps('key')}
          />
          <Button
            loading={isCreatingMessage}
            variant="outline"
            color="red"
            type="submit"
            style={{ marginTop: '16px' }}>Push</Button>
          <Button
            variant="default"
            type="button"
            onClick={closeAddMessage}
            style={{ marginTop: '16px', marginLeft: '8px' }}>Cancel</Button>
        </form>
      </Modal>
      {selectedTopic?.name ? (
        <>
          <Anchor component="button" onClick={() => setSelectedTopic(null)} ta="left">
            Back to topics
          </Anchor>
          <Title order={3}>
            Topic: {selectedTopic.name}
          </Title>
          <Box style={{ overflowY: 'auto', border: '1px solid lightgray' }}>
            <ResourceViewToolbar>
              <ResourceViewToolbarItem
                label="Produce message"
                leftSection={<IconMessageUp color={theme.colors[theme.primaryColor][4]} height={16} />}
                onClick={openAddMessage}
              />
            </ResourceViewToolbar>
            <KafkaTopicConsumer topicName={selectedTopic.name} resourceId={resourceId} />
          </Box>
          <DataTable
            borderRadius="sm"
            withColumnBorders
            withTableBorder={false}
            highlightOnHover
            records={selectedTopic.partitions || []}
            columns={[
              { accessor: 'partition', title: 'Partition' },
              { accessor: 'leader', title: 'Leader' },
              {
                accessor: 'replicas',
                title: 'Replicas',
                render: ({ replicas }) => replicas?.join(', '),
              },
              { accessor: 'isr', title: 'ISR', render: ({ isr }) => isr?.join(', ') },
              { accessor: 'elr', title: 'ELR' },
              { accessor: 'lastKnownElr', title: 'Last Known ELR' },
            ]}
          />
        </>
      ) : (
        <>
          <ResourceViewToolbar>
            <ResourceViewToolbarItem
              label="Add Topic"
              leftSection={<IconPlus color={theme.colors[theme.primaryColor][4]} height={16} />}
              onClick={openAddTopic}
            />
          </ResourceViewToolbar>
          <DataTable
            borderRadius="sm"
            withColumnBorders
            withTableBorder={false}
            highlightOnHover
            records={kafkaTopics || []}
            columns={[
              { accessor: 'name', title: 'Name' },
              { accessor: 'topicId', title: 'Topic ID' },
              { accessor: 'partitionCount', title: 'Partition count' },
              { accessor: 'replicationFactor', title: 'Replication factor' },
            ]}
            onRowClick={({ record }) => !selectedTopic && setSelectedTopic(record as KafkaTopicDto)}
          />
        </>
      )}
    </Stack>
  );
};
