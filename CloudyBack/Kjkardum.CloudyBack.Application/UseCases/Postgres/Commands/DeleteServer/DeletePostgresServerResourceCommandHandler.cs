using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.DeleteServer;

public class DeletePostgresServerResourceCommandHandler(
    IGeneralContainerStatusClient client,
    IBaseResourceRepository baseResourceRepository,
    IPostgresServerResourceRepository repository) : IRequestHandler<DeletePostgresServerResourceCommand>
{
    public async Task Handle(DeletePostgresServerResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await repository.GetById(request.Id);
        if (resource == null)
        {
            throw new EntityNotFoundException($"Postgres server resource with ID {request.Id} not found.");
        }
        await repository.Delete(resource);

        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(DeletePostgresServerResourceCommand),
                ActionDisplayText = $"Delete resource {resource.Name}",
                ResourceId = resource.ResourceGroupId
            });
        await client.DeleteContainerAsync(request.Id);
    }
}
