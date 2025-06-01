using FluentValidation;

namespace Kjkardum.CloudyBack.Application.UseCases.WebApplication.Commands.Create;

public class CreateWebApplicationResourceCommandValidator: AbstractValidator<CreateWebApplicationResourceCommand>
{
    public CreateWebApplicationResourceCommandValidator()
    {
        RuleFor(x => x.ResourceGroupId).NotEmpty();
        RuleFor(x => x.WebApplicationName)
            .Must(x => !x.Contains(' '))
            .WithMessage("Name cannot contain spaces")
            .Must(x => !"123456789".Contains(x[0]))
            .WithMessage("Name cannot start with a number")
            .MaximumLength(100);
    }
}
