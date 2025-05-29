namespace Kjkardum.CloudyBack.Application.UseCases.Kafka.Dtos;

public class KafkaTopicDto
{
    public string Name { get; set; }
    public string TopicId { get; set; }
    public int PartitionCount { get; set; }
    public int ReplicationFactor { get; set; }
    public List<KafkaPartitionDto> Partitions { get; set; } = new();
}

public class KafkaPartitionDto
{
    public string Topic { get; set; }
    public int Partition { get; set; }
    public int Leader { get; set; }
    public List<int> Replicas { get; set; } = new();
    public List<int> Isr { get; set; } = new();
    public string Elr { get; set; }
    public string LastKnownElr { get; set; }
}
