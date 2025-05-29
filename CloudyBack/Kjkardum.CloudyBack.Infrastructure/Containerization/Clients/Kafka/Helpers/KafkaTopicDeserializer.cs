using Kjkardum.CloudyBack.Application.UseCases.Kafka.Dtos;

namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.Kafka.Helpers;

public static class KafkaTopicDeserializer
{
    private static readonly char[] LineSeparators = ['\n', '\r'];
    private static readonly char[] KeyValueSeparators = [':'];

    public static List<KafkaTopicDto> ParseKafkaTopicsOutput(string input)
    {
        var topics = new List<KafkaTopicDto>();
        KafkaTopicDto? currentTopic = null;

        var lines = input.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            if (!line.StartsWith('\t')) // Topic header line
            {
                var parts = line.Split('\t');
                var topic = new KafkaTopicDto();

                foreach (var part in parts)
                {
                    var kv = part.Split(KeyValueSeparators, 2);
                    if (kv.Length != 2)
                    {
                        continue;
                    }

                    var key = kv[0].Trim();
                    var value = kv[1].Trim();

                    switch (key)
                    {
                        case "Topic":
                            topic.Name = value;
                            break;
                        case "TopicId":
                            topic.TopicId = value;
                            break;
                        case "PartitionCount":
                            topic.PartitionCount = int.Parse(value);
                            break;
                        case "ReplicationFactor":
                            topic.ReplicationFactor = int.Parse(value);
                            break;
                    }
                }

                topics.Add(topic);
                currentTopic = topic;
            }
            else if (line.Trim().StartsWith("Topic:")) // Partition line
            {
                var parts = line.Trim().Split('\t');
                var partition = new KafkaPartitionDto();

                foreach (var part in parts)
                {
                    var kv = part.Split(KeyValueSeparators, 2);
                    if (kv.Length != 2)
                    {
                        continue;
                    }

                    var key = kv[0].Trim();
                    var value = kv[1].Trim();

                    switch (key)
                    {
                        case "Topic":
                            partition.Topic = value;
                            break;
                        case "Partition":
                            partition.Partition = int.Parse(value);
                            break;
                        case "Leader":
                            partition.Leader = int.Parse(value);
                            break;
                        case "Replicas":
                            partition.Replicas = value
                                .Split(',')
                                .Where(v => !string.IsNullOrWhiteSpace(v))
                                .Select(int.Parse)
                                .ToList();
                            break;
                        case "Isr":
                            partition.Isr = value
                                .Split(',')
                                .Where(v => !string.IsNullOrWhiteSpace(v))
                                .Select(int.Parse)
                                .ToList();
                            break;
                        case "Elr":
                            partition.Elr = value;
                            break;
                        case "LastKnownElr":
                            partition.LastKnownElr = value;
                            break;
                    }
                }

                currentTopic?.Partitions.Add(partition);
            }
        }

        return topics;
    }
}
