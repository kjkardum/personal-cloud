namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;

public class ResourceGroupedBaseResourceDto: BaseResourceDto
{
    public Guid ResourceGroupId { get; set; }
    public string ResourceGroupName { get; set; }
}
