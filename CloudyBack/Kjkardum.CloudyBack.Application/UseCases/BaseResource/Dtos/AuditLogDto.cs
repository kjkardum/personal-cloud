namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;

public class AuditLogDto
{
    public Guid Id { get; set; }
    public required string ActionName { get; set; }
    public required string ActionDisplayText { get; set; }
    public string? ActionMetadata { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
