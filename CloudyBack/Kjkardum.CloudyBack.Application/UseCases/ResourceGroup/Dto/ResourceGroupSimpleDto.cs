namespace Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Dto;

public class ResourceGroupSimpleDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
