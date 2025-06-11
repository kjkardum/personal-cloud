using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Helpers;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.RunDatabaseQuery;

public class RunPostgresDatabaseQueryCommandHandler(
    IPostgresDatabaseResourceRepository postgresDatabaseRepository,
    IPostgresServerClient postgresServerClient)
    : IRequestHandler<RunPostgresDatabaseQueryCommand, PostgresQueryResultDto>
{
    public async Task<PostgresQueryResultDto> Handle(
        RunPostgresDatabaseQueryCommand request,
        CancellationToken cancellationToken)
    {
        var resource = await postgresDatabaseRepository.GetById(request.DatabaseId);
        if (resource == null)
        {
            throw new EntityNotFoundException($"Postgres database with ID {request.DatabaseId} not found.");
        }

        if (PostgresQuerySanitizer.ContainsRoleOrSessionSet(request.Query))
        {
            throw new ForbiddenAccessException("Setting role or session authorization is not allowed in this query.");
        }
        var resultCsv = await postgresServerClient.RunQueryAsync(
            resource.PostgresDatabaseServerResourceId,
            resource.PostgresDatabaseServerResource!.SaUsername,
            resource.PostgresDatabaseServerResource!.SaPassword,
            resource.Name,
            resource.AdminUsername,
            request.Query);

        var result = PostgresCsvConverter.ToQueryDto(resultCsv, asSa: false);
        return result;
    }
}
