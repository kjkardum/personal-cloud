using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Infrastructure.Persistence.Configuration;

public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(t => t.Id);

        builder.HasIndex(t => t.Email).IsUnique();

        builder.Property(t => t.Email).HasColumnType("nvarchar(100)");
        builder.Property(t => t.PasswordHash).HasColumnType("nvarchar(100)");
    }
}
