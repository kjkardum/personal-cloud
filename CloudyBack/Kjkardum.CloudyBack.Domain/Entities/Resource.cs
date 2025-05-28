namespace Kjkardum.CloudyBack.Domain.Entities;

public abstract class BaseResource: IAuditableEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<AuditLogEntry> AuditLogEntries { get; set; } = new List<AuditLogEntry>();
}

public class AuditLogEntry
{
    public Guid Id { get; set; }
    public required string ActionName { get; set; }
    public required string ActionDisplayText { get; set; }
    public string? ActionMetadata { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Guid ResourceId { get; set; }
    public BaseResource? Resource { get; set; }
}

public class ResourceGroup: BaseResource
{
    public List<ResourceGroupedBaseResource>? Resources { get; set; }
}

public abstract class ResourceGroupedBaseResource: BaseResource
{
    public Guid ResourceGroupId { get; set; }
    public ResourceGroup? ResourceGroup { get; set; }
}

public class PostgresServerResource: ResourceGroupedBaseResource
{
    public required string SaUsername { get; set; }
    public required string SaPassword { get; set; }

    public List<PostgresDatabaseResource>? PostgresDatabaseResources { get; set; }
    public int Port { get; set; }
}

public class PostgresDatabaseResource: ResourceGroupedBaseResource
{
    public required string DatabaseName { get; set; }
    public required string AdminUsername { get; set; }
    public required string AdminPassword { get; set; }

    public Guid PostgresDatabaseServerResourceId { get; set; }
    public PostgresServerResource? PostgresDatabaseServerResource { get; set; }
}

public class KafkaClusterResource: ResourceGroupedBaseResource
{
    public required string SaUsername { get; set; }
    public required string SaPassword { get; set; }
    public int Port { get; set; }
}
