namespace Kjkardum.CloudyBack.Application.Clients;

public interface IPostgresServerClient
{
    Task CreateServerAsync(Guid id, int serverPort, string saUsername, string saPassword);
    Task CreateDatabaseAsync(Guid id, string saUsername, string saPassword, string databaseName, string dbUsername, string dbPassword);
}
