namespace Kjkardum.CloudyBack.Domain.Entities;

public interface IAuditableEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
