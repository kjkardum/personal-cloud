using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.CreateServer;

public class CreatePostgresServerCommand: IRequest<PostgresServerResourceDto>
{
    public required Guid ResourceGroupId { get; set; }
    public required string ServerName { get; set; }
    public int? ServerPort { get; set; }
}
