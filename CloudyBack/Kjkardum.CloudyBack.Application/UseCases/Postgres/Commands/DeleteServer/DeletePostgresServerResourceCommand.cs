using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.DeleteServer;

public class DeletePostgresServerResourceCommand: IRequest
{
    public Guid Id { get; set; }
}
