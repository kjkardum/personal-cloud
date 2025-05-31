using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Kafka.Commands.CreateTopic;

public class CreateKafkaTopicCommandHandler(
    IKafkaRepository kafkaRepository,
    IBaseResourceRepository baseResourceRepository,
    IKafkaClient kafkaClient) : IRequestHandler<CreateKafkaTopicCommand>
{
    public async Task Handle(CreateKafkaTopicCommand request, CancellationToken cancellationToken)
    {
        var kafkaClusterResource = await kafkaRepository.GetById(request.ServerId);
        if (kafkaClusterResource is null)
        {
            throw new KeyNotFoundException($"Kafka cluster with id {request.ServerId} not found.");
        }

        await kafkaClient.CreateTopicAsync(
            request.ServerId,
            request.TopicName);

        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(CreateKafkaTopicCommand),
                ActionDisplayText = $"Create topic {request.TopicName} on server {request.ServerId}",
                ResourceId = request.ServerId
            });
    }
}
