using FluentValidation;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.CreateServer;

public class CreatePostgresServerCommandValidator: AbstractValidator<CreatePostgresServerCommand>
{
    public CreatePostgresServerCommandValidator()
    {
        RuleFor(x => x.ResourceGroupId).NotEmpty();
        RuleFor(x => x.ServerName)
            .Must(x => !x.Contains(' '))
            .WithMessage("Name cannot contain spaces")
            .Must(x => !"123456789".Contains(x[0]))
            .WithMessage("Name cannot start with a number")
            .MaximumLength(100);
    }
}
