using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.AddDatabaseExtension;

public class AddPostgresDatabaseExtensionCommandHandler(
    IPostgresDatabaseResourceRepository postgresDatabaseResourceRepository,
    IPostgresServerClient postgresServerClient,
    IBaseResourceRepository baseResourceRepository,
    ILogger<AddPostgresDatabaseExtensionCommandHandler> logger) : IRequestHandler<AddPostgresDatabaseExtensionCommand>
{
    public async Task Handle(AddPostgresDatabaseExtensionCommand request, CancellationToken cancellationToken)
    {
        var resource = await postgresDatabaseResourceRepository.GetById(request.DatabaseId);
        if (resource == null)
        {
            throw new EntityNotFoundException($"Postgres database with ID {request.DatabaseId} not found.");
        }

        // Run the command to add the extension
        var resultMsg = await postgresServerClient.RunQueryAsync(
            resource.PostgresDatabaseServerResourceId,
            resource.PostgresDatabaseServerResource!.SaUsername,
            resource.PostgresDatabaseServerResource!.SaPassword,
            resource.Name,
            null,
            $"CREATE EXTENSION IF NOT EXISTS \"{request.ExtensionName}\";");

        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(AddPostgresDatabaseExtensionCommand),
                ActionDisplayText = $"Added extension '{request.ExtensionName}' to Postgres database '{resource.Name}'",
                ResourceId = request.DatabaseId,
            });

        logger.LogInformation(
            "Try add extension '{ExtensionName}' to Postgres database {ServerId}. Result: {ResultMsg}",
            request.ExtensionName,
            request.DatabaseId,
            resultMsg);
    }
}
