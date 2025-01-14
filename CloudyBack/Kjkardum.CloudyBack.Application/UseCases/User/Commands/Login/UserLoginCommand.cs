using MediatR;
using Kjkardum.CloudyBack.Application.UseCases.User.Dto;

namespace Kjkardum.CloudyBack.Application.UseCases.User.Commands.Login;

public record UserLoginCommand: IRequest<LoggedInUserDto>
{
    public string Email { get; set; } = null!;

    public string NormalizedEmail
        => Email.Trim().ToLowerInvariant();
    public string Password { get; set; } = null!;
}
