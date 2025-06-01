using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.VirtualNetwork.Commands.Create;

public class CreateVirtualNetworkResourceCommand: IRequest<VirtualNetworkSimpleDto>
{
    public Guid ResourceGroupId { get; set; }
    public string Name { get; set; }
}
