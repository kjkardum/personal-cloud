using AutoMapper;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.VirtualNetwork.Commands.Create;

public class CreateVirtualNetworkResourceCommandHandler(
    IVirtualNetworkRepository virtualNetworkRepository,
    IBaseResourceRepository baseResourceRepository,
    IVirtualNetworkClient virtualNetworkClient,
    IMapper mapper) : IRequestHandler<CreateVirtualNetworkResourceCommand, VirtualNetworkSimpleDto>
{
    public async Task<VirtualNetworkSimpleDto> Handle(
        CreateVirtualNetworkResourceCommand request,
        CancellationToken cancellationToken)
    {
        var virtualNetwork = new VirtualNetworkResource {
            Name = request.Name,
            ResourceGroupId = request.ResourceGroupId };
        var createdVirtualNetwork = await virtualNetworkRepository.Create(virtualNetwork);
        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(CreateVirtualNetworkResourceCommand),
                ActionDisplayText = $"Create new virtual network {request.Name}",
                ResourceId = createdVirtualNetwork.Id
            });
        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(CreateVirtualNetworkResourceCommand),
                ActionDisplayText = $"Create new resource {request.Name}",
                ResourceId = request.ResourceGroupId
            });
        await virtualNetworkClient.CreateVirtualNetworkAsync(createdVirtualNetwork.Id);
        var mappedVirtualNetwork = mapper.Map<VirtualNetworkSimpleDto>(createdVirtualNetwork);
        return mappedVirtualNetwork;
    }
}
