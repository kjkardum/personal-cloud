namespace Kjkardum.CloudyBack.Application.Clients;

public interface IPostgresServerClient
{
    Task CreateServerAsync(Guid id, int serverPort, string saUsername, string saPassword, string serverName);
    Task CreateDatabaseAsync(Guid id, string saUsername, string saPassword, string databaseName, string dbUsername, string dbPassword);
    Task StartServerAsync(Guid requestId);
    Task StopServerAsync(Guid requestId);
    Task RestartServerAsync(Guid requestId);
    Task<string> RunQueryAsync(Guid serverId, string saUsername, string saPassword, string database, string? impersonateUser, string requestQuery);
}
