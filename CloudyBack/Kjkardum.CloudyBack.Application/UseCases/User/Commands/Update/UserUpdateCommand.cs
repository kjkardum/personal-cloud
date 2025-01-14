using MediatR;
using System.Text.Json.Serialization;

namespace Kjkardum.CloudyBack.Application.UseCases.User.Commands.Update;

public class UserUpdateCommand: IRequest
{
    [JsonIgnore] public Guid RequestorId { get; set; }
    [JsonIgnore] public Guid Id { get; set; }
    public string? NewPassword { get; set; }
}
