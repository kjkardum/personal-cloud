using FluentValidation;

namespace Kjkardum.CloudyBack.Application.UseCases.User.Commands.Login;

public class UserLoginCommandValidator: AbstractValidator<UserLoginCommand>
{
    public UserLoginCommandValidator()
    {
        RuleFor(t => t.Email)
            .EmailAddress();
        RuleFor(t => t.Password)
            .NotEmpty();
    }
}
