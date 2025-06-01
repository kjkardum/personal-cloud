using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.Kafka.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Kafka.Queries.GetTopics;

public class GetKafkaTopicsByClusterIdQueryHandler(
    IKafkaServiceRepository kafkaServiceRepository,
    IKafkaClient kafkaClient) : IRequestHandler<GetKafkaTopicsByClusterIdQuery, List<KafkaTopicDto>>
{
    public async Task<List<KafkaTopicDto>> Handle(
        GetKafkaTopicsByClusterIdQuery request,
        CancellationToken cancellationToken)
    {
        var kafkaClusterResource = await kafkaServiceRepository.GetById(request.Id);
        if (kafkaClusterResource is null)
        {
            throw new KeyNotFoundException($"Kafka cluster with id {request.Id} not found.");
        }

        var topics = await kafkaClient.GetTopicsAsync(request.Id);
        return topics;
    }
}
