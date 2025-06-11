using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.AddDatabaseExtension;

public class AddPostgresDatabaseExtensionCommand: IRequest
{
    public Guid ServerId { get; set; }
    public string ExtensionName { get; set; }
}
