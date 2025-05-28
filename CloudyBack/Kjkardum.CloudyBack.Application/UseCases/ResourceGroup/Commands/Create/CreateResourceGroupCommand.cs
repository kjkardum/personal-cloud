using Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Dto;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Commands.Create;

public class CreateResourceGroupCommand: IRequest<ResourceGroupSimpleDto>
{
    public required string Name { get; set; }
}
