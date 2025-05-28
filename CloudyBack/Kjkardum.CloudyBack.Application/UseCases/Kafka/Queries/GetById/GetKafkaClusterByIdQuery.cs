using Kjkardum.CloudyBack.Application.UseCases.Kafka.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Kafka.Queries.GetById;

public class GetKafkaClusterByIdQuery: IRequest<KafkaClusterResourceDto>
{
    public Guid Id { get; set; }
}
