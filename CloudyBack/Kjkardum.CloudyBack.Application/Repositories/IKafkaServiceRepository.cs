using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Repositories;

public interface IKafkaServiceRepository
{
    Task<KafkaClusterResource> Create(KafkaClusterResource kafkaClusterResource);
    Task<KafkaClusterResource?> GetById(Guid id);
    Task Delete(KafkaClusterResource resource);
}
