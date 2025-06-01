using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using MediatR;
using System.Runtime.CompilerServices;

namespace Kjkardum.CloudyBack.Application.UseCases.Kafka.Queries.ConsumeMessages;

public class KafkaConsumeMessagesQueryHandler(
    IKafkaServiceRepository kafkaServiceRepository,
    IKafkaClient kafkaClient) : IStreamRequestHandler<KafkaConsumeMessagesQuery, string>
{
    public async IAsyncEnumerable<string> Handle(
        KafkaConsumeMessagesQuery request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var kafkaClusterResource = await kafkaServiceRepository.GetById(request.Id);
        if (kafkaClusterResource is null)
        {
            throw new KeyNotFoundException($"Kafka cluster with id {request.Id} not found.");
        }

        var messages = kafkaClient.StreamTopicMessagesAsync(
            request.Id,
            request.Topic,
            cancellationToken: cancellationToken);

        await foreach (var message in messages)
        {
            yield return message;
        }
    }
}
