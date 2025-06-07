using MediatR;
using System.Text.Json.Serialization;

namespace Kjkardum.CloudyBack.Application.UseCases.ReverseProxy.Commands.Disconnect;

public class DisconnectReverseProxyCommand: IRequest
{
    [JsonIgnore] public Guid ResourceId { get; set; }
    public Guid ConnectionId { get; set; }
}
