using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;
using System.Text.Json.Serialization;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.CreateDatabase;

public class CreatePostgresDatabaseCommand: IRequest<PostgresDatabaseSimpleResourceDto>
{
    [JsonIgnore] public Guid ServerId { get; set; }
    public required string DatabaseName { get; set; }
    public required string AdminUsername { get; set; }
    public required string AdminPassword { get; set; }
}
