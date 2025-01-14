using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Kjkardum.CloudyBack.Integration.Tests;

public class WebHostTestBase: IDisposable
{
    public HttpClient Client { get; set; }
    public string JwToken { get; set; } = string.Empty;
    public Dictionary<string, string?> CachedValues { get; set; }
    public WebHostTestBase()
    {
        var webApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.ConfigureAppConfiguration(
                config => config.AddConfiguration(MockDefaultConfiguration())));
        SqlConnection = new SqliteConnection("DataSource=file::memory:?cache=shared");
        SqlConnection.Open();
        Client = webApplicationFactory.CreateClient();
        CachedValues = new Dictionary<string, string?>();
    }

    public SqliteConnection SqlConnection { get; set; }

    protected static IConfiguration MockDefaultConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "ASPNETCORE_URLS", "https://localhost:5000;http://localhost:6000" },
            { "ConnectionStrings:DefaultConnection", "TestConnectionString" },
        };

        var configurationBuilder = new ConfigurationBuilder();
        var configuration = configurationBuilder.AddInMemoryCollection(inMemorySettings).Build();

        return configuration;
    }

    public void Dispose()
    {
        Client.Dispose();
        SqlConnection.Dispose();
    }
}
