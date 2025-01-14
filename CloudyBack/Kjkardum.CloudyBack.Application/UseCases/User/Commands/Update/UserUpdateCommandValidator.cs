using FluentValidation;

namespace Kjkardum.CloudyBack.Application.UseCases.User.Commands.Update;

public class UserUpdateCommandValidator: AbstractValidator<UserUpdateCommand>
{
    public UserUpdateCommandValidator()
    {

        RuleFor(x => x.NewPassword)
            .MinimumLength(8)
            .MaximumLength(50)
            .When(t => t.NewPassword != null);
    }
}
