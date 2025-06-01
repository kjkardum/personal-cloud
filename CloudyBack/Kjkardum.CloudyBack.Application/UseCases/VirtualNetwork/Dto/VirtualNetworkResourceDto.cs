using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;

namespace Kjkardum.CloudyBack.Application.UseCases.VirtualNetwork.Dto;

public class VirtualNetworkResourceDto: ResourceGroupedBaseResourceDto
{
    public ICollection<BaseResourceDto> Resources { get; set; }
}
