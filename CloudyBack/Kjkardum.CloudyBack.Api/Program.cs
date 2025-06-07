using Hellang.Middleware.ProblemDetails;
using System.Reflection;
using Kjkardum.CloudyBack.Api.Extensions;
using Kjkardum.CloudyBack.Api.Services;
using Kjkardum.CloudyBack.Application;
using Kjkardum.CloudyBack.Infrastructure;
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

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseProblemDetails();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

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
}
