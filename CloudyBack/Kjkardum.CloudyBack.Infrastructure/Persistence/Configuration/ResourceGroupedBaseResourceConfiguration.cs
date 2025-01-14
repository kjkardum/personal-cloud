using Kjkardum.CloudyBack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kjkardum.CloudyBack.Infrastructure.Persistence.Configuration;

public class ResourceGroupedBaseResourceConfiguration: IEntityTypeConfiguration<ResourceGroupedBaseResource>
{
    public void Configure(EntityTypeBuilder<ResourceGroupedBaseResource> builder) =>
        builder.HasOne(e => e.ResourceGroup)
            .WithMany(e => e.Resources)
            .HasForeignKey(e => e.ResourceGroupId)
            .OnDelete(DeleteBehavior.Restrict);
}
