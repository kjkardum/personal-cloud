using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Kafka.Commands.ClusterContainerAction;

public class KafkaClusterContainerActionCommandHandler(
    IBaseResourceRepository baseResourceRepository,
    IKafkaClient kafkaClient): IRequestHandler<KafkaClusterContainerActionCommand>
{
    public async Task Handle(KafkaClusterContainerActionCommand request, CancellationToken cancellationToken)
    {
        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(KafkaClusterContainerActionCommand),
                ActionDisplayText = $"Trigger docker action {request.ActionId} on server",
                ResourceId = request.Id
            });
        switch (request.ActionId)
        {
            case "start":
                await kafkaClient.StartServerAsync(request.Id);
                break;
            case "stop":
                await kafkaClient.StopServerAsync(request.Id);
                break;
            case "restart":
                await kafkaClient.RestartServerAsync(request.Id);
                break;
            default:
                throw new EntityNotFoundException("Invalid action id");
        }
    }
}
