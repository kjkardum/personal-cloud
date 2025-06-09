using Docker.DotNet;
using Docker.DotNet.Models;
using Hellang.Middleware.ProblemDetails;
using System.Reflection;
using Kjkardum.CloudyBack.Api.Extensions;
using Kjkardum.CloudyBack.Api.Services;
using Kjkardum.CloudyBack.Application;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Infrastructure;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Helpers;
using Kjkardum.CloudyBack.Infrastructure.Persistence;
using Kjkardum.CloudyBack.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
ConfigureServices(services, builder.Configuration, builder.Environment);

var app = builder.Build();

// Seed the database
await FillDatabase(app.Services, builder.Configuration);
await CreateRequiredContainer(app.Services, builder.Configuration);

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseProblemDetails();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapWhen(
    context => !context.Request.Path.StartsWithSegments("/api"),
    appBuilder => {
        appBuilder.UseStaticFiles();
        appBuilder.UseRouting();
        appBuilder.UseEndpoints(endpoints => endpoints.MapFallbackToFile("index.html"));
    });

app.UseCors(corsBuilder => corsBuilder
    .WithOrigins(
        "http://cloudy.local:3000",
        "http://localhost:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());

app.Run();

public partial class Program
{
    public static void ConfigureServices(
        IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services
            .AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.AddEndpointsApiExplorer();
        services.AddLogging();

        services.AddSwagger();

        services.AddApplication(configuration);
        services.AddInfrastructure(configuration);

        services.AddHttpContextAccessor();

        services.AddJwtAuthorization(configuration);

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddCustomProblemDetailsResponses(environment);

        services.AddScoped<IAuthenticationService, AuthenticationService>();
    }

    public static async Task FillDatabase(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<ApplicationDbContext>();
        if (configuration.GetConnectionString("DefaultConnection") == "TestConnectionString")
        {
            await context.Database.EnsureCreatedAsync();
        }
        else
        {
            await context.Database.MigrateAsync();
        }

        await TenantAndUserSeed.Seed(context);
    }

    public static async Task CreateRequiredContainer(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        var inDocker = configuration.GetValue<bool>("ApplicationConfiguration__InDocker");
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        var observabilityClient = services.GetRequiredService<IObservabilityClient>();
        await observabilityClient.EnsureCreated();
        var reverseProxyClient = services.GetRequiredService<IReverseProxyClient>();
        await reverseProxyClient.EnsureCreated();

        var dockerClient = services.GetRequiredService<DockerClient>();
        if (inDocker)
        {
            var selfContainer = await dockerClient.Containers.InspectContainerAsync("cloudyadminapp");
            if (selfContainer.NetworkSettings.Networks.ContainsKey(DockerNamingHelper.ObservabilityNetworkName))
            {
                logger.LogInformation("Cloudy observability network already exists");
            }
            else
            {
                await dockerClient.Networks.ConnectNetworkAsync(
                    DockerNamingHelper.ObservabilityNetworkName,
                    new NetworkConnectParameters
                    {
                        Container = selfContainer.ID
                    });
            }
        }
        try
        {
            var lokiPlugin = await dockerClient.Plugin.InspectPluginAsync("loki");
            if (!lokiPlugin.Enabled)
            {
                await dockerClient.Plugin.EnablePluginAsync(
                    "loki",
                    new PluginEnableParameters
                        {
                            Timeout = 30
                        });
                logger.LogInformation("Cloudy loki is enabled");
            }
        }
        catch
        {
            var lokiPluginPrivilege = await dockerClient.Plugin.GetPluginPrivilegesAsync(
                new PluginGetPrivilegeParameters {Remote = "grafana/loki-docker-driver:3.3.2-amd64"})
                ?? new List<PluginPrivilege>();
            await dockerClient.Plugin.InstallPluginAsync(
                new PluginInstallParameters
                {
                    Remote = "grafana/loki-docker-driver:3.3.2-amd64",
                    Name = "loki",
                    Privileges = lokiPluginPrivilege
                },
                new Progress<JSONMessage>());
            logger.LogInformation("Cloudy loki plugin installed successfully");
            var plugin = await dockerClient.Plugin.InspectPluginAsync("cloudy-loki-plugin");
            if (!plugin.Enabled)
            {
                await dockerClient.Plugin.EnablePluginAsync("cloudy-loki-plugin",  new PluginEnableParameters
                {
                    Timeout = 30
                });
                logger.LogInformation("Cloudy loki plugin enabled successfully");
            }
        }
    }
}
