using FluentValidation;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.CreateDatabase;

public class CreatePostgresDatabaseCommandValidator: AbstractValidator<CreatePostgresDatabaseCommand>
{
    private const string AdminCannotBeSa = "Admin username cannot be 'sa', 'admin' or other common names";
    private static readonly string[] ForbiddenAdminNames = new[] {"sa", "admin", "administrator", "postgres"};

    public CreatePostgresDatabaseCommandValidator()
    {
        RuleFor(x => x.ServerId).NotEmpty();
        RuleFor(x => x.DatabaseName)
            .Must(x => !x.Contains(' '))
            .WithMessage("Name cannot contain spaces")
            .Must(x => !x.Contains('\''))
            .WithMessage("Name cannot contain apostrophes")
            .Must(x => !"123456789".Contains(x[0]))
            .WithMessage("Name cannot start with a number")
            .Matches("[a-zA-Z0-9_]*$")
            .WithMessage("Name can only contain alphanumeric characters and underscores")
            .MaximumLength(100);
        RuleFor(x => x.AdminUsername)
            .NotEmpty()
            .Must(x => !ForbiddenAdminNames.Contains(x, StringComparer.OrdinalIgnoreCase))
            .WithMessage(AdminCannotBeSa)
            .Must(x => !x.Contains(' '))
            .WithMessage("Admin username cannot contain spaces")
            .Must(x => !x.Contains('\''))
            .WithMessage("Admin username cannot contain apostrophes");
        RuleFor(x => x.AdminPassword)
            .Must(x => !x.Contains('\''))
            .WithMessage("Admin password cannot contain apostrophes")
            .NotEmpty()
            .MinimumLength(8);
    }
}
