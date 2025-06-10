namespace Kjkardum.CloudyBack.Domain.Entities;

public class User: IAuditableEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public bool SuperAdmin { get; set; }
    public DateTime LastLogin { get; set; }
}
