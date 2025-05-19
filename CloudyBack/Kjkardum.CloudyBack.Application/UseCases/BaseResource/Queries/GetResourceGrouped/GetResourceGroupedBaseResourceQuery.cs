using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetResourceGrouped;

public class GetResourceGroupedBaseResourceQuery: IRequest<ResourceGroupedBaseResourceDto>
{
    public Guid Id { get; set; }
}
