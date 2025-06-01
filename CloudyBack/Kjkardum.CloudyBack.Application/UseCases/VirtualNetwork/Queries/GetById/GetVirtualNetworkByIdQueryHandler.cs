using ApiExceptions.Exceptions;
using AutoMapper;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.VirtualNetwork.Dto;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.VirtualNetwork.Queries.GetById;

public class GetVirtualNetworkByIdQueryHandler(
    IVirtualNetworkRepository virtualNetworkRepository,
    IMapper mapper)
    : IRequestHandler<GetVirtualNetworkByIdQuery, VirtualNetworkResourceDto>
{
    public async Task<VirtualNetworkResourceDto> Handle(
        GetVirtualNetworkByIdQuery request,
        CancellationToken cancellationToken)
    {
        var virtualNetwork = await virtualNetworkRepository.GetById(request.Id);
        if (virtualNetwork == null)
        {
            throw new EntityNotFoundException($"Virtual network with ID {request.Id} not found.");
        }

        return mapper.Map<VirtualNetworkResourceDto>(virtualNetwork);
    }
}
