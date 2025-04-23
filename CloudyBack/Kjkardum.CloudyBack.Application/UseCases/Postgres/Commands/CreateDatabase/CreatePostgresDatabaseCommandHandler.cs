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
    IMapper mapper,
    IPostgresServerClient client) : IRequestHandler<CreatePostgresDatabaseCommand, PostgresDatabaseResourceDto>
{
    public async Task<PostgresDatabaseResourceDto> Handle(
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


        return mapper.Map<PostgresDatabaseResourceDto>(databaseResource);
    }
}
