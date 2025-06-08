using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.ServerContainerAction;

internal class PostgresServerContainerActionCommandHandler(
    IBaseResourceRepository baseResourceRepository,
    IPostgresServerClient postgresServerClient) : IRequestHandler<PostgresServerContainerActionCommand>
{
    public async Task Handle(PostgresServerContainerActionCommand request, CancellationToken cancellationToken)
    {
        var resource = await baseResourceRepository.GetById(request.Id);
        if (resource is not PostgresServerResource)
        {
            throw new EntityNotFoundException($"Postgres server with id {request.Id} not found.");
        }
        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(PostgresServerContainerActionCommand),
                ActionDisplayText = $"Trigger docker action {request.ActionId} on server",
                ResourceId = request.Id
            });
        switch (request.ActionId)
        {
            case "start":
                await postgresServerClient.StartServerAsync(request.Id);
                break;
            case "stop":
                await postgresServerClient.StopServerAsync(request.Id);
                break;
            case "restart":
                await postgresServerClient.RestartServerAsync(request.Id);
                break;
            default:
                throw new EntityNotFoundException("Invalid action id");
        }
    }
}
