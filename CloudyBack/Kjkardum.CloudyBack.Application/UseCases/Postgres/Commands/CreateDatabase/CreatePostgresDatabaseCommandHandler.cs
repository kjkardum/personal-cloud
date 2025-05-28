using AutoMapper;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.CreateDatabase;

public class CreatePostgresDatabaseCommandHandler(
    IPostgresServerResourceRepository serverRepository,
    IPostgresDatabaseResourceRepository databaseRepository,
    IBaseResourceRepository baseResourceRepository,
    IMapper mapper,
    IPostgresServerClient client) : IRequestHandler<CreatePostgresDatabaseCommand, PostgresDatabaseSimpleResourceDto>
{
    public async Task<PostgresDatabaseSimpleResourceDto> Handle(
        CreatePostgresDatabaseCommand request,
        CancellationToken cancellationToken)
    {
        var serverResource = await serverRepository.GetById(request.ServerId);
        if (serverResource is null)
        {
            throw new Exception($"Postgres server with id {request.ServerId} not found.");
        }

        var databaseResource = new PostgresDatabaseResource
        {
            Name = request.DatabaseName,
            ResourceGroupId = serverResource.ResourceGroupId,
            DatabaseName = request.DatabaseName,
            AdminUsername = request.AdminUsername,
            AdminPassword = request.AdminPassword,
            PostgresDatabaseServerResourceId = serverResource.Id,
        };

        await databaseRepository.Create(databaseResource);

        await client.CreateDatabaseAsync(
            serverResource.Id,
            serverResource.SaUsername,
            serverResource.SaPassword,
            request.DatabaseName,
            request.AdminUsername,
            request.AdminPassword);

        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(CreatePostgresDatabaseCommand),
                ActionDisplayText = $"Create new database {request.DatabaseName} on server {serverResource.Name}",
                ResourceId = serverResource.Id
            });
        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(CreatePostgresDatabaseCommand),
                ActionDisplayText = $"Create new database {request.DatabaseName} on server {serverResource.Name}",
                ResourceId = databaseResource.Id
            });

        return mapper.Map<PostgresDatabaseSimpleResourceDto>(databaseResource);
    }
}
