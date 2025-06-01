namespace Kjkardum.CloudyBack.Application.Clients;

public interface IWebApplicationClient
{
    Task BuildAndRunWebApplicationUsingGitRepo(
        Guid id,
        string repoUrl,
        string buildCommand,
        string runCommand,
        int port,
        List<string> environmentVariables,
        List<Guid> virtualNetworks);
}
