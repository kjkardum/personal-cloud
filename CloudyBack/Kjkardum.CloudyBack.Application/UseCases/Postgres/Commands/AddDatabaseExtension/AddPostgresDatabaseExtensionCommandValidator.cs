using FluentValidation;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.AddDatabaseExtension;

public class AddPostgresDatabaseExtensionCommandValidator: AbstractValidator<AddPostgresDatabaseExtensionCommand>
{
    public AddPostgresDatabaseExtensionCommandValidator()
    {
        RuleFor(command => command.ExtensionName)
            .NotEmpty()
            .WithMessage("Extension name cannot be empty.")
            .Matches(@"^[a-zA-Z0-9_]+$")
            .WithMessage("Extension name can only contain alphanumeric characters and underscores.");
    }
}
