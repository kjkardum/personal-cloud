using Kjkardum.CloudyBack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kjkardum.CloudyBack.Infrastructure.Persistence.Configuration;

public class AuditLogEntryConfiguration: IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.HasIndex(e => e.Timestamp);
        builder.HasIndex(e =>
            new {
                e.ResourceId, e.Timestamp
            })
            .IsDescending(false, true)
            .IsUnique();
    }
}
