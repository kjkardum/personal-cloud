using MediatR;
using System.Text.Json.Serialization;

namespace Kjkardum.CloudyBack.Application.UseCases.ReverseProxy.Commands.Connect;

public class ConnectReverseProxyCommand: IRequest
{
    [JsonIgnore] public Guid ResourceId { get; set; }
    public string UrlForHost { get; set; }
    public bool UseHttps { get; set; }
}
