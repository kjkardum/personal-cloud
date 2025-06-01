using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Kafka.Commands.ProduceTopicMessage;

public class ProduceKafkaTopicMessageCommandHandler(
    IKafkaServiceRepository kafkaServiceRepository,
    IBaseResourceRepository baseResourceRepository,
    IKafkaClient kafkaClient) : IRequestHandler<ProduceKafkaTopicMessageCommand>
{
    public async Task Handle(ProduceKafkaTopicMessageCommand request, CancellationToken cancellationToken)
    {
        var kafkaClusterResource = await kafkaServiceRepository.GetById(request.ServerId);
        if (kafkaClusterResource is null)
        {
            throw new KeyNotFoundException($"Kafka cluster with id {request.ServerId} not found.");
        }

        await kafkaClient.ProduceMessageAsync(
            request.ServerId,
            request.Topic,
            request.Value,
            request.Key);

        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(ProduceKafkaTopicMessageCommand),
                ActionDisplayText = $"Produce message to topic {request.Topic} on server {request.ServerId}",
                ResourceId = request.ServerId
            });
    }
}
