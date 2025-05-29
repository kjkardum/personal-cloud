using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Kafka.Commands.ClusterContainerAction;

public class KafkaClusterContainerActionCommand: IRequest
{
    public Guid Id { get; set; }
    public string ActionId { get; set; }
}
