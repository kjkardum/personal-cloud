using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.VirtualNetwork.Commands.Connect;

public class ConnectVirtualNetworkCommand: IRequest
{
    public Guid NetworkId { get; set; }
    public Guid ResourceId { get; set; }
}
