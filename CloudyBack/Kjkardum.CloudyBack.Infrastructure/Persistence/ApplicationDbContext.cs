using Kjkardum.CloudyBack.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Infrastructure.Persistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<KeyValueTableEntry> KeyValueTableEntries => Set<KeyValueTableEntry>();
        public DbSet<BaseResource> Resources => Set<BaseResource>();
        public DbSet<AuditLogEntry> AuditLogEntries => Set<AuditLogEntry>();
        public DbSet<ResourceGroupedBaseResource> ResourceGroupedBaseResources => Set<ResourceGroupedBaseResource>();
        public DbSet<VirtualNetworkResource> VirtualNetworkResources => Set<VirtualNetworkResource>();
        public DbSet<VirtualNetworkConnection> VirtualNetworkConnections => Set<VirtualNetworkConnection>();
        public DbSet<PublicProxyConfiguration> PublicProxyConfigurations => Set<PublicProxyConfiguration>();
        public DbSet<VirtualNetworkableBaseResource> VirtualNetworkableBaseResources
            => Set<VirtualNetworkableBaseResource>();

        public DbSet<ResourceGroup> ResourceGroups => Set<ResourceGroup>();
        public DbSet<PostgresServerResource> PostgresServerResources => Set<PostgresServerResource>();
        public DbSet<PostgresDatabaseResource> PostgresDatabaseResources => Set<PostgresDatabaseResource>();
        public DbSet<KafkaClusterResource> KafkaClusterResources => Set<KafkaClusterResource>();
        public DbSet<WebApplicationResource> WebApplicationResources => Set<WebApplicationResource>();
        public DbSet<WebApplicationConfigurationEntry> WebApplicationConfigurationEntries
            => Set<WebApplicationConfigurationEntry>();

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
