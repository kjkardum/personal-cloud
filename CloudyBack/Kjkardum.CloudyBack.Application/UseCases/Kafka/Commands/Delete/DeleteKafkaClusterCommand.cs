using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Kafka.Commands.Delete;

public class DeleteKafkaClusterCommand: IRequest
{
    public Guid Id { get; set; }
}
