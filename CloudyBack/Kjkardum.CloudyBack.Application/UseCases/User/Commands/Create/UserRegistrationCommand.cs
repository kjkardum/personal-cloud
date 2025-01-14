using MediatR;
using System.Text.Json.Serialization;

namespace Kjkardum.CloudyBack.Application.UseCases.User.Commands.Create;

public record UserRegistrationCommand: IRequest
{
    [JsonIgnore] public Guid RequestorId { get; set; }
    public string Email { get; set; } = null!;

    public string NormalizedEmail
        => Email.Trim().ToUpperInvariant();
    public string Password { get; set; } = null!;
}
