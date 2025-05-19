using AutoMapper;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;
using System.Security.Cryptography;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.CreateServer;

public class CreatePostgresServerCommandHandler(
    IPostgresServerResourceRepository repository,
    IBaseResourceRepository baseResourceRepository,
    IPostgresServerClient postgresServerClient,
    IPrometheusClient prometheusClient,
    IMapper mapper) : IRequestHandler<CreatePostgresServerCommand, PostgresServerResourceDto>
{
    public async Task<PostgresServerResourceDto> Handle(CreatePostgresServerCommand request, CancellationToken cancellationToken)
    {
        //geenerate cryptographically secure random string
        var passwordBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(passwordBytes);
        }

        var password = Convert.ToBase64String(passwordBytes);
        var random = new Random();
        var postgresServerResource = new PostgresServerResource
        {
            Name = request.ServerName,
            ResourceGroupId = request.ResourceGroupId,
            Port = request.ServerPort ?? random.Next(49152, 65535),
            SaUsername = "CloudyAdmin",
            SaPassword = password
        };

        var createdPostgresServerResource = await repository.Create(postgresServerResource);

        await prometheusClient.AttachCollector(createdPostgresServerResource.Id, request.ServerName);
        await postgresServerClient.CreateServerAsync(
            postgresServerResource.Id,
            postgresServerResource.Port,
            postgresServerResource.SaUsername,
            postgresServerResource.SaPassword,
            postgresServerResource.Name);

        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(CreatePostgresServerCommand),
                ActionDisplayText = $"Create new postgres server {request.ServerName}",
                ResourceId = createdPostgresServerResource.Id
            });

        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(CreatePostgresServerCommand),
                ActionDisplayText = $"Create new resource {request.ServerName}",
                ResourceId = request.ResourceGroupId
            });


        var mappedPostgresServerResource = mapper.Map<PostgresServerResourceDto>(createdPostgresServerResource);

        return mappedPostgresServerResource;
    }
}
