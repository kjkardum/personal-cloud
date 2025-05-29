using AutoMapper;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.Kafka.Dtos;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;
using System.Security.Cryptography;

namespace Kjkardum.CloudyBack.Application.UseCases.Kafka.Commands.Create;

public class CreateKafkaClusterCommandHandler(
    IKafkaRepository repository,
    IBaseResourceRepository baseResourceRepository,
    IKafkaClient kafkaClient,
    IObservabilityClient observabilityClient,
    IMapper mapper): IRequestHandler<CreateKafkaClusterCommand, KafkaClusterResourceDto>
{
    public async Task<KafkaClusterResourceDto> Handle(
        CreateKafkaClusterCommand request,
        CancellationToken cancellationToken)
    {
        var passwordBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(passwordBytes);
        }

        var password = Convert.ToBase64String(passwordBytes);
        var random = new Random();
        var kafkaClusterResource = new KafkaClusterResource
        {
            Name = request.ServerName,
            ResourceGroupId = request.ResourceGroupId,
            Port = request.ServerPort ?? random.Next(49152, 65535),
            SaUsername = "CloudyAdmin",
            SaPassword = password
        };
        kafkaClusterResource = await repository.Create(kafkaClusterResource);

        await observabilityClient.AttachCollector(kafkaClusterResource.Id, request.ServerName);

        await kafkaClient.CreateClusterAsync(
            kafkaClusterResource.Id,
            kafkaClusterResource.Name);

        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(CreateKafkaClusterCommand),
                ActionDisplayText = $"Create new Kafka cluster {request.ServerName}",
                ResourceId = kafkaClusterResource.Id
            });

        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(CreateKafkaClusterCommand),
                ActionDisplayText = $"Create new resource {request.ServerName}",
                ResourceId = request.ResourceGroupId
            });

        return mapper.Map<KafkaClusterResourceDto>(kafkaClusterResource);
    }
}
