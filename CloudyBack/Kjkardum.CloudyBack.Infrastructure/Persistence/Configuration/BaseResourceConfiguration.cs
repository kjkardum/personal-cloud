using Kjkardum.CloudyBack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kjkardum.CloudyBack.Infrastructure.Persistence.Configuration;

public class BaseResourceConfiguration: IEntityTypeConfiguration<BaseResource>
{
    public void Configure(EntityTypeBuilder<BaseResource> builder)
    {
        builder.HasIndex(e => e.Name).IsUnique();
        builder.HasIndex(t => t.CreatedAt);
        builder.HasIndex(t => t.UpdatedAt);

        builder.UseTpcMappingStrategy();
    }
}
