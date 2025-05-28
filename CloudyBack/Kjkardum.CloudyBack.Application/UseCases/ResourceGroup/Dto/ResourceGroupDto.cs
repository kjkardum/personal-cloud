using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;

namespace Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Dto;

public class ResourceGroupDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<BaseResourceDto> Resources { get; set; }
}
