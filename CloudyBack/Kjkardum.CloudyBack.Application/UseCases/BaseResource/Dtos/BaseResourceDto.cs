namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;

public class BaseResourceDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public required string ResourceType { get; set; }
}
