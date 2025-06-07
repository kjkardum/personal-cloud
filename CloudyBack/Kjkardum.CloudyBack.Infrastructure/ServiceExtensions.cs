using Docker.DotNet;
using Kjkardum.CloudyBack.Application.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.Services;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.General;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.Kafka;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.Orchestration;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.Postgres;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.ReverseProxy;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.VirtualNetwork;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.WebApplication;
using Kjkardum.CloudyBack.Infrastructure.Identity;
using Kjkardum.CloudyBack.Infrastructure.Persistence;
using Kjkardum.CloudyBack.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;

namespace Kjkardum.CloudyBack.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (connectionString != "TestConnectionString")
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
            else
            {
                options.UseSqlite(new SqliteConnection("DataSource=file::memory:?cache=shared"));
            }
        });

        services.AddSingleton<DockerClient>(_ => new DockerClientConfiguration().CreateClient());
        services.AddTransient<IGeneralContainerStatusClient, GeneralContainerStatusClient>();
        services.AddTransient<IObservabilityClient, ObservabilityClient>();
        services.AddHttpClient("Prometheus", c => c.BaseAddress = new Uri("http://localhost:9090"));
        services.AddHttpClient("Loki", c => c.BaseAddress = new Uri("http://localhost:3100/loki/"));
        services.AddTransient<IPostgresServerClient, PostgresServerClient>();
        services.AddTransient<IKafkaClient, KafkaClient>();
        services.AddTransient<IWebApplicationClient, WebApplicationClient>();
        services.AddTransient<IVirtualNetworkClient, VirtualNetworkClient>();
        services.AddTransient<IReverseProxyClient, CaddyReverseProxyClient>();

        services.AddTransient<ISignInService, SignInService>();

        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IBaseResourceRepository, BaseResourceRepository>();
        services.AddTransient<IResourceGroupedBaseResourceRepository, ResourceGroupedBaseResourceRepository>();
        services.AddTransient<IResourceGroupRepository, ResourceGroupRepository>();
        services.AddTransient<IPostgresServerResourceRepository, PostgresServerResourceRepository>();
        services.AddTransient<IPostgresDatabaseResourceRepository, PostgresDatabaseResourceRepository>();
        services.AddTransient<IKafkaServiceRepository, KafkaServiceRepository>();
        services.AddTransient<IWebApplicationResourceRepository, WebApplicationResourceRepository>();
        services.AddTransient<IVirtualNetworkRepository, VirtualNetworkRepository>();
        services.AddTransient<IVirtualNetworkableBaseResourceRepository, VirtualNetworkableBaseResourceRepository>();
        return services;
    }
}
