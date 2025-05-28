using AutoMapper;
using Kjkardum.CloudyBack.Application.UseCases.Kafka.Dtos;
using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Mappings;

public class KafkaProfile: Profile
{
    public KafkaProfile()
    {
        CreateMap<KafkaClusterResource, KafkaClusterResourceDto>();
    }
}
