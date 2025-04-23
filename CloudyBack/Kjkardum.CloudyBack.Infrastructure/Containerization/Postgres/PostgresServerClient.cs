using Docker.DotNet;
using Docker.DotNet.Models;
using Kjkardum.CloudyBack.Application.Clients;
using Microsoft.Extensions.Logging;

namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Postgres;

public class PostgresServerClient(DockerClient client, ILogger<PostgresServerClient> logger) : IPostgresServerClient
{
    private const string ImageName = "postgres";

    public async Task CreateServerAsync(Guid id, int serverPort, string saUsername, string saPassword)
    {
        await client.Images.CreateImageAsync(
            new ImagesCreateParameters { FromImage = ImageName, Tag = "latest" },
            null,
            new Progress<JSONMessage>());
        logger.LogInformation("Image pulled");
        await client.Volumes.CreateAsync(new VolumesCreateParameters { Name = $"cloudy{id:N}volume" });
        logger.LogInformation("Volume created");
        await client.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Name = $"cloudy{id:N}container",
                Image = ImageName,
                HostConfig = new HostConfig
                {
                    Binds = new List<string> { $"{id:N}volume:/var/lib/postgresql/data", },
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        { "5432/tcp", new List<PortBinding> { new() { HostPort = $"{serverPort}" } } }
                    },
                },
                ExposedPorts = new Dictionary<string, EmptyStruct> { { "5432/tcp", default } },
                Env = new List<string> { $"POSTGRES_PASSWORD={saPassword}", $"POSTGRES_USER={saUsername}" },
            });
        logger.LogInformation("Container created");
        await client.Containers.StartContainerAsync($"cloudy{id:N}container", new ContainerStartParameters());
        logger.LogInformation("Container started");
    }

    public async Task CreateDatabaseAsync(
        Guid id,
        string saUsername,
        string saPassword,
        string databaseName,
        string dbUsername,
        string dbPassword)
    {
        var container = await client.Containers.InspectContainerAsync($"cloudy{id:N}container");
        logger.LogInformation("Container found");
        var DatabaseCreate = $"CREATE DATABASE {databaseName};";
        var UserCreateAndGrant = $"CREATE USER {dbUsername} WITH PASSWORD '{dbPassword}';" +
            $"GRANT ALL PRIVILEGES ON DATABASE {databaseName} TO {dbUsername};" +
            $"ALTER USER {dbUsername} SET search_path TO {databaseName};" +
            $"REVOKE ALL ON DATABASE postgres FROM PUBLIC;";
        var ConnectToNewDatabase = $"\\connect {databaseName};";
        var CreateSchema = $"CREATE SCHEMA IF NOT EXISTS {databaseName};" +
            $"REVOKE ALL ON DATABASE {databaseName} FROM PUBLIC;" +
            $"GRANT ALL PRIVILEGES ON SCHEMA {databaseName} TO {dbUsername};" +
            $"ALTER DATABASE {databaseName} SET search_path TO {databaseName};";
        var exec = await client.Exec.ExecCreateContainerAsync(
            container.ID,
            new ContainerExecCreateParameters
                {
                    AttachStdin = true,
                    AttachStdout = true,
                    AttachStderr = true,
                    Env = new List<string>
                    {
                        $"PGPASSWORD={saPassword}"
                    },
                    Cmd = new List<string>
                    {
                        "psql", "-U", saUsername,
                        "-P", "pager=off", "-P", "format=csv",
                        "-c", DatabaseCreate,
                        "-c", UserCreateAndGrant,
                        "-c", ConnectToNewDatabase,
                        "-c", CreateSchema
                    },
                    Tty = true,
                    Detach = false
                });
        logger.LogInformation("Exec created");
        var execStart = await client.Exec.StartAndAttachContainerExecAsync(exec.ID, true);
        var (stdOut, stdErr) = await execStart.ReadOutputToEndAsync(default);
        logger.LogInformation("Response: {StdOut}", stdOut);
    }
}
