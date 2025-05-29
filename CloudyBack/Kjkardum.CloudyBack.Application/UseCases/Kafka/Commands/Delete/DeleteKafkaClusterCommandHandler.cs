using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Kafka.Commands.Delete;

public class DeleteKafkaClusterCommandHandler(
    IGeneralContainerStatusClient client,
    IBaseResourceRepository baseResourceRepository,
    IKafkaRepository kafkaRepository): IRequestHandler<DeleteKafkaClusterCommand>
{
    public async Task Handle(DeleteKafkaClusterCommand request, CancellationToken cancellationToken)
    {
        var resource = await kafkaRepository.GetById(request.Id);
        if (resource == null)
        {
            throw new EntityNotFoundException($"Postgres server resource with ID {request.Id} not found.");
        }
        await kafkaRepository.Delete(resource);

        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(DeleteKafkaClusterCommand),
                ActionDisplayText = $"Delete resource {resource.Name}",
                ResourceId = resource.ResourceGroupId
            });
        await client.DeleteContainerAsync(request.Id);
    }
}
