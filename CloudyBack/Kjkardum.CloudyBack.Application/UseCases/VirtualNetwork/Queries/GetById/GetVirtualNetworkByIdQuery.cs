using Kjkardum.CloudyBack.Application.UseCases.VirtualNetwork.Dto;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.VirtualNetwork.Queries.GetById;

public class GetVirtualNetworkByIdQuery: IRequest<VirtualNetworkResourceDto>
{
    public Guid Id { get; set; }
}
