using FluentValidation;

namespace Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Commands.Create;

public class CreateResourceGroupCommandValidator: AbstractValidator<CreateResourceGroupCommand>
{
    public CreateResourceGroupCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Must(x => !x.Contains(' ')).WithMessage("Name cannot contain spaces")
            .Must(x => !"123456789".Contains(x[0])).WithMessage("Name cannot start with a number")
            .MaximumLength(100);
    }
}
