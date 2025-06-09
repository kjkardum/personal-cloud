using Kjkardum.CloudyBack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kjkardum.CloudyBack.Infrastructure.Persistence.Configuration;

public class KeyValueTableEntryConfiguration: IEntityTypeConfiguration<KeyValueTableEntry>
{
    public void Configure(EntityTypeBuilder<KeyValueTableEntry> builder) => builder.HasKey(kv => kv.Key);
}
