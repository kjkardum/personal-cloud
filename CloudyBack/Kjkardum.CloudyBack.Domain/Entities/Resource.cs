using Kjkardum.CloudyBack.Domain.Enums;

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

public class VirtualNetworkResource: ResourceGroupedBaseResource
{
    public ICollection<VirtualNetworkConnection>? NetworkConnections { get; set; }
}

public class VirtualNetworkConnection
{
    public Guid Id { get; set; }
    public Guid VirtualNetworkId { get; set; }
    public VirtualNetworkResource? VirtualNetwork { get; set; }
    public Guid ResourceId { get; set; }
    public VirtualNetworkableBaseResource? Resource { get; set; }
}

public abstract class VirtualNetworkableBaseResource : ResourceGroupedBaseResource
{
    public ICollection<VirtualNetworkConnection>? VirtuaLNetworks { get; set; }
}

public class PostgresServerResource: VirtualNetworkableBaseResource
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

public class KafkaClusterResource: VirtualNetworkableBaseResource
{
    public required string SaUsername { get; set; }
    public required string SaPassword { get; set; }
    public int Port { get; set; }
}

public class WebApplicationResource: VirtualNetworkableBaseResource
{
    public string SourcePath { get; set; }
    public WebApplicationSourceType SourceType { get; set; }
    public string BuildCommand { get; set; } = string.Empty;
    public string StartupCommand { get; set; } = string.Empty;
    public string HealthCheckUrl { get; set; } = string.Empty;
    public int Port { get; set; }
    public ICollection<WebApplicationConfigurationEntry>? Configuration { get; set; }
}

public class WebApplicationConfigurationEntry
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public Guid WebApplicationResourceId { get; set; }
    public WebApplicationResource? WebApplicationResource { get; set; }
}
