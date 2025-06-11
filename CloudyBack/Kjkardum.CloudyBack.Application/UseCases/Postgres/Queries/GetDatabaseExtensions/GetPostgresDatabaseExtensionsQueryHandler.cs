using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Helpers;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Queries.GetDatabaseExtensions;

public class GetPostgresDatabaseExtensionsQueryHandler(
    IPostgresServerResourceRepository postgresServerRepository,
    IPostgresServerClient postgresServerClient)
    : IRequestHandler<GetPostgresDatabaseExtensionsQuery, PostgresQueryResultDto>
{
    public async Task<PostgresQueryResultDto> Handle(
        GetPostgresDatabaseExtensionsQuery request,
        CancellationToken cancellationToken)
    {
        var resource = await postgresServerRepository.GetById(request.ServerId);
        if (resource == null)
        {
            throw new EntityNotFoundException($"Postgres server with ID {request.ServerId} not found.");
        }
        var extensionsCsv = await postgresServerClient.RunQueryAsync(
            resource.Id,
            resource.SaUsername,
            resource.SaPassword,
            "postgres",
            null,
            "SELECT name, installed_version FROM pg_available_extensions;");

        var result = PostgresCsvConverter.ToQueryDto(extensionsCsv, asSa: true);
        return result;
    }
}
