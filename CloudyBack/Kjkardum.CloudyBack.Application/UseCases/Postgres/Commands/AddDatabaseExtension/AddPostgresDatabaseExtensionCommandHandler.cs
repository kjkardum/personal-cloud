using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.AddDatabaseExtension;

public class AddPostgresDatabaseExtensionCommandHandler(
    IPostgresServerResourceRepository postgresServerRepository,
    IPostgresServerClient postgresServerClient,
    ILogger<AddPostgresDatabaseExtensionCommandHandler> logger) : IRequestHandler<AddPostgresDatabaseExtensionCommand>
{
    public async Task Handle(AddPostgresDatabaseExtensionCommand request, CancellationToken cancellationToken)
    {
        var resource = await postgresServerRepository.GetById(request.ServerId);
        if (resource == null)
        {
            throw new EntityNotFoundException($"Postgres server with ID {request.ServerId} not found.");
        }

        // Run the command to add the extension
        var resultMsg = await postgresServerClient.RunQueryAsync(
            resource.Id,
            resource.SaUsername,
            resource.SaPassword,
            "postgres",
            null,
            $"CREATE EXTENSION IF NOT EXISTS \"{request.ExtensionName}\";");

        logger.LogInformation(
            "Try add extension '{ExtensionName}' to Postgres server {ServerId}. Result: {ResultMsg}",
            request.ExtensionName,
            request.ServerId,
            resultMsg);
    }
}
