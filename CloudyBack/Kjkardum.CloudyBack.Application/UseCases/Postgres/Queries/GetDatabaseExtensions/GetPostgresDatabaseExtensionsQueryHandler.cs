using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Helpers;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Queries.GetDatabaseExtensions;

public class GetPostgresDatabaseExtensionsQueryHandler(
    IPostgresDatabaseResourceRepository postgresDatabaseResourceRepository,
    IPostgresServerClient postgresServerClient,
    IBaseResourceRepository baseResourceRepository)
    : IRequestHandler<GetPostgresDatabaseExtensionsQuery, PostgresQueryResultDto>
{
    public async Task<PostgresQueryResultDto> Handle(
        GetPostgresDatabaseExtensionsQuery request,
        CancellationToken cancellationToken)
    {
        var resource = await postgresDatabaseResourceRepository.GetById(request.DatabaseId);
        if (resource == null)
        {
            throw new EntityNotFoundException($"Postgres database with ID {request.DatabaseId} not found.");
        }
        var extensionsCsv = await postgresServerClient.RunQueryAsync(
            resource.PostgresDatabaseServerResourceId,
            resource.PostgresDatabaseServerResource!.SaUsername,
            resource.PostgresDatabaseServerResource!.SaPassword,
            resource.Name,
            null,
            "SELECT name, installed_version FROM pg_available_extensions;");

        var result = PostgresCsvConverter.ToQueryDto(extensionsCsv, asSa: true);
        return result;
    }
}
