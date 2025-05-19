using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.ServerContainerAction;

public class PostgresServerContainerActionCommand: IRequest
{
    public Guid Id { get; set; }
    public string ActionId { get; set; }
}
