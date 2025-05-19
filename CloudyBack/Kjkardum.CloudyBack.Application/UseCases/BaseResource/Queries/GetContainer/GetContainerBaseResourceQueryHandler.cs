using AutoMapper;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetContainer;

public class GetContainerBaseResourceQueryHandler(IGeneralContainerStatusClient client, IMapper mapper)
    : IRequestHandler<GetContainerBaseResourceQuery, ContainerDto>
{
    public async Task<ContainerDto> Handle(GetContainerBaseResourceQuery request, CancellationToken cancellationToken)
    {
        var container = await client.GetContainerStateAsync(request.Id);
        var containerDto = mapper.Map<ContainerDto>(container);
        return containerDto;
    }
}
