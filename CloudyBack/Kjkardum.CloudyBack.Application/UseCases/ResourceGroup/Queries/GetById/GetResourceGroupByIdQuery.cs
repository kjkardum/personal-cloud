using Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Dto;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Queries.GetById;

public class GetResourceGroupByIdQuery: IRequest<ResourceGroupDto>
{
    public Guid Id { get; set; }
}
