using FluentValidation;

namespace Kjkardum.CloudyBack.Application.UseCases.User.Commands.Create;

public class UserRegistrationCommandValidator: AbstractValidator<UserRegistrationCommand>
{
    public UserRegistrationCommandValidator()
    {
        RuleFor(t => t.Email)
            .EmailAddress();

        RuleFor(t => t.Password)
            .MinimumLength(8)
            .MaximumLength(32);
    }
}
