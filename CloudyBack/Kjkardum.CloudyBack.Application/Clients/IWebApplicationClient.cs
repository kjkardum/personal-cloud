using Kjkardum.CloudyBack.Domain.Enums;

namespace Kjkardum.CloudyBack.Application.Clients;

public interface IWebApplicationClient
{
    Task BuildAndRunWebApplicationUsingGitRepo(
        Guid id,
        string name,
        string repoUrl,
        string buildCommand,
        string runCommand,
        WebApplicationRuntimeType runtimeType,
        int port,
        List<string> environmentVariables,
        List<Guid> virtualNetworks);

    Task StartServerAsync(Guid requestId);
    Task StopServerAsync(Guid requestId);
    Task RestartServerAsync(Guid requestId);
}
