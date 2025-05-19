using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetContainer;

public class GetContainerBaseResourceQuery: IRequest<ContainerDto>
{
    public Guid Id { get; set; }
}
