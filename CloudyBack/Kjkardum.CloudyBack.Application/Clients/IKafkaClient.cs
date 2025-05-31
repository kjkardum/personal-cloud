using Kjkardum.CloudyBack.Application.UseCases.Kafka.Dtos;

namespace Kjkardum.CloudyBack.Application.Clients;

public interface IKafkaClient
{
    Task CreateClusterAsync(Guid id, string clusterName);
    Task<List<KafkaTopicDto>> GetTopicsAsync(Guid id);
    Task StartServerAsync(Guid id);
    Task StopServerAsync(Guid id);
    Task RestartServerAsync(Guid id);

    IAsyncEnumerable<string> StreamTopicMessagesAsync(
        Guid id,
        string topicName,
        CancellationToken cancellationToken = default);

    Task CreateTopicAsync(Guid id, string topicName);

    Task ProduceMessageAsync(Guid id, string topicName, string message, string? key = null);
}
