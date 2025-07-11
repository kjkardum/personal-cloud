using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.AddDatabaseExtension;

public class AddPostgresDatabaseExtensionCommand: IRequest
{
    public Guid DatabaseId { get; set; }
    public string ExtensionName { get; set; }
}
