using Kjkardum.CloudyBack.Application.UseCases.Kafka.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Kafka.Queries.GetTopics;

public class GetKafkaTopicsByClusterIdQuery: IRequest<List<KafkaTopicDto>>
{
    public Guid Id { get; set; }
}
