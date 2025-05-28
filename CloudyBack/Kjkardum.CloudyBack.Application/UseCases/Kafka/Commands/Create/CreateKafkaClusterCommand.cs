using Kjkardum.CloudyBack.Application.UseCases.Kafka.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Kafka.Commands.Create;

public class CreateKafkaClusterCommand: IRequest<KafkaClusterResourceDto>
{
    public required Guid ResourceGroupId { get; set; }
    public required string ServerName { get; set; }
    public int? ServerPort { get; set; }
}
