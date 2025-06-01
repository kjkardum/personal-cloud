using ApiExceptions.Exceptions;
using AutoMapper;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.Kafka.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Kafka.Queries.GetById;

public class GetKafkaClusterByIdQueryHandler(
    IKafkaServiceRepository kafkaServiceRepository,
    IMapper mapper) : IRequestHandler<GetKafkaClusterByIdQuery, KafkaClusterResourceDto>
{
    public async Task<KafkaClusterResourceDto> Handle(
        GetKafkaClusterByIdQuery request,
        CancellationToken cancellationToken)
    {
        var kafkaClusterResource = await kafkaServiceRepository.GetById(request.Id);
        if (kafkaClusterResource is null)
        {
            throw new EntityNotFoundException($"Kafka cluster with id {request.Id} not found.");
        }
        return mapper.Map<KafkaClusterResourceDto>(kafkaClusterResource);
    }
}
