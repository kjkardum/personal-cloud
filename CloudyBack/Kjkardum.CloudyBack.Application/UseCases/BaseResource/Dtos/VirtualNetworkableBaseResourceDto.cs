namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;

public class VirtualNetworkableBaseResourceDto: ResourceGroupedBaseResourceDto
{
    public ICollection<VirtualNetworkSimpleDto> VirtualNetworks { get; set; }
    public ICollection<PublicProxyConfigurationDto> PublicProxyConfigurations { get; set; }
}
