using Kjkardum.CloudyBack.Application.UseCases.Kafka.Dtos;

namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.Kafka.Helpers;

using Kjkardum.CloudyBack.Application.UseCases.Kafka.Dtos;
using System.Text.RegularExpressions;

public static partial class KafkaTopicDeserializer
{
    public static List<KafkaTopicDto> ParseKafkaTopicsOutput(string output)
    {
        var topics = new List<KafkaTopicDto>();
        KafkaTopicDto? currentTopic = null;
        var partitionRegex = PartitionRegex();

        foreach (var line in output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmedLine = line.Trim();

            // Match a new topic
            if (trimmedLine.StartsWith("topic \""))
            {
                // Finish any existing topic
                if (currentTopic != null)
                {
                    topics.Add(currentTopic);
                }

                // Parse the topic name and initial partition count
                var topicMatch = TopicNameAndCountRegex().Match(trimmedLine);
                if (topicMatch.Success)
                {
                    currentTopic = new KafkaTopicDto
                    {
                        Name = topicMatch.Groups["name"].Value,
                        PartitionCount = int.Parse(topicMatch.Groups["partitions"].Value),
                        ReplicationFactor = 1, // KafkaCat doesn't directly show replication factor here, default to 1
                        Partitions = new List<KafkaPartitionDto>()
                    };
                }
            }
            // Match partitions
            else if (currentTopic != null)
            {
                var partitionMatch = partitionRegex.Match(trimmedLine);
                if (partitionMatch.Success)
                {
                    var partitionId = int.Parse(partitionMatch.Groups["partition"].Value);
                    var leader = int.Parse(partitionMatch.Groups["leader"].Value);

                    var replicas = partitionMatch.Groups["replicas"].Value
                        .Split(',')
                        .Select(s => int.Parse(s.Trim()))
                        .ToList();

                    var isrs = partitionMatch.Groups["isrs"].Value
                        .Split(',')
                        .Select(s => int.Parse(s.Trim()))
                        .ToList();

                    currentTopic.Partitions.Add(new KafkaPartitionDto
                        {
                            Topic = currentTopic.Name,
                            Partition = partitionId,
                            Leader = leader,
                            Replicas = replicas,
                            Isr = isrs
                        });
                }
            }
        }

        // Add the last topic if one was in progress
        if (currentTopic != null)
        {
            topics.Add(currentTopic);
        }

        return topics;
    }

    [GeneratedRegex(
        @"^\s*partition\s+(?<partition>\d+),\s+leader\s+(?<leader>\d+),\s+replicas:\s+(?<replicas>[\d, ]+),\s+isrs:\s+(?<isrs>[\d, ]+)",
        RegexOptions.Compiled)]
    private static partial Regex PartitionRegex();
    [GeneratedRegex(@"^topic\s+""(?<name>[^""]+)""\s+with\s+(?<partitions>\d+)\s+partitions:")]
    private static partial Regex TopicNameAndCountRegex();
}
