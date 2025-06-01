using ApiExceptions.Exceptions;
using Docker.DotNet;
using Docker.DotNet.Models;
using ICSharpCode.SharpZipLib.Tar;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Helpers;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text;

namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.WebApplication;

public class WebApplicationClient(DockerClient client, ILogger<WebApplicationClient> logger): IWebApplicationClient
{
    // on build i will (delete if exists) and create volume for web application source
    // i will mount that volume to the WebApplicationBuilder.Dockerfile container to /appsrc (this is already workdir of dockerfile)
    // i will run the container with command of git clone <repo-url> && bash -c "<command method argument to build the app>"
    // on run i will (delete if exists) and create container for this web application and mount /appsrc to the container's /appsrc
    // i will run the container with command cd /appsrc && <command method argument to run the app>
    public async Task BuildAndRunWebApplicationUsingGitRepo(
        Guid id,
        string repoUrl,
        string buildCommand,
        string runCommand,
        int port,
        List<string> environmentVariables,
        List<Guid> virtualNetworks)
    {
        logger.LogInformation("Building and running web application from repository: {RepoUrl}", repoUrl);

        await BuildWebApplicationUsingGitRepo(id, repoUrl, buildCommand, environmentVariables, virtualNetworks);
        await RunWebApplicationUsingGitRepo(id, runCommand, port, environmentVariables, virtualNetworks);
    }

    private async Task RunWebApplicationUsingGitRepo(Guid id, string runCommand, int port, List<string> environmentVariables, List<Guid> virtualNetworks)
    {
        //TODO offer runtime image as a parameter
        //TODO offer port as a parameter, TODO dont expose port as there will be a reverse proxy in front of this
        const string tempRunnerImageName = "python:bookworm";
        var volumeName = DockerNamingHelper.GetVolumeName(id);
        var appContainer = DockerNamingHelper.GetContainerName(id);
        logger.LogInformation("Running web application with ID: {Id} using command: {RunCommand}", id, runCommand);
        await RemoveExistingContainer(id);
        await client.Images.CreateImageAsync(
            new ImagesCreateParameters { FromImage = tempRunnerImageName, Tag = "latest" },
            null,
            new Progress<JSONMessage>());
        var containerParameters = new CreateContainerParameters
        {
            Image = tempRunnerImageName,
            Name = appContainer,
            HostConfig = new HostConfig
            {
                Binds = [ $"{volumeName}:/appsrc" ],
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { $"{port}/tcp", new List<PortBinding> { new() { HostPort = "8080" } } }
                }
            },
            NetworkingConfig = new NetworkingConfig
            {
                EndpointsConfig = virtualNetworks.ToDictionary(
                    DockerNamingHelper.GetVirtualNetworkResourceName,
                    _ => new EndpointSettings())
            },
            Env = environmentVariables,
            ExposedPorts = new Dictionary<string, EmptyStruct> { { $"{port}/tcp", default } },
            Cmd = new List<string> { "bash", "-c", $"cd /appsrc && {runCommand}" }
        };
        var containerReference = await client.Containers.CreateContainerAsync(containerParameters);
        await client.Containers.StartContainerAsync(containerReference.ID, new ContainerStartParameters());
    }

    private async Task RemoveExistingContainer(Guid id)
    {
        var appContainer = DockerNamingHelper.GetContainerName(id);
        try
        {
            var existingContainer = await client.Containers.InspectContainerAsync(appContainer);
            logger.LogInformation("Removing existing container: {ContainerName}", appContainer);
            await client.Containers.RemoveContainerAsync(
                appContainer,
                new ContainerRemoveParameters { Force = true });
        }
        catch (Exception ex)
        {
            logger.LogInformation("No existing container found with name: {ContainerName}", appContainer);
        }
    }

    public async Task<string> RecreateVolumeForWebApplication(Guid id)
    {
        var volumeName = DockerNamingHelper.GetVolumeName(id);
        logger.LogInformation("Recreating volumeName for web application: {VolumeName}", volumeName);
        try
        {
            await client.Volumes.RemoveAsync(volumeName, true);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to remove volume {VolumeName}. It may not exist.", volumeName);
        }
        await client.Volumes.CreateAsync(new VolumesCreateParameters { Name = volumeName });
        return volumeName;
    }

    public async Task BuildWebApplicationUsingGitRepo(
        Guid id,
        string repoUrl,
        string buildCommand,
        List<string> environmentVariables,
        List<Guid> virtualNetworks)
    {
        logger.LogInformation(
            "Building web application with ID: {Id} from repository: {RepoUrl} using command: {BuildCommand}", id,
            repoUrl, buildCommand);
        await RemoveExistingContainer(id);
        var volumeName = await RecreateVolumeForWebApplication(id);
        var builderImageName = await GetBuilderDockerImage();
        var container = new CreateContainerParameters
        {
            Image = builderImageName,
            Name = DockerNamingHelper.GetContainerName(id)+ "builder",
            HostConfig = new HostConfig
            {
                Binds = [ $"{volumeName}:/appsrc" ],
            },
            NetworkingConfig = new NetworkingConfig
            {
                EndpointsConfig = virtualNetworks.ToDictionary(
                    DockerNamingHelper.GetVirtualNetworkResourceName,
                    _ => new EndpointSettings())
            },
            Env = environmentVariables,
            Cmd = new List<string>()
            {
                "bash",
                "-c",
                string.IsNullOrWhiteSpace(buildCommand) ?
                    $"git clone {repoUrl} ."
                    : $"git clone {repoUrl} . && {buildCommand}"
            }
        };
        var builderContainerReference = await client.Containers.CreateContainerAsync(container);
        try
        {
            logger.LogInformation("Created container for building web application: {ContainerId}", builderContainerReference.ID);
            await client.Containers.StartContainerAsync(builderContainerReference.ID, new ContainerStartParameters());
            logger.LogInformation("Started container for building web application: {ContainerId}", builderContainerReference.ID);
            var waitTask = client.Containers.WaitContainerAsync(builderContainerReference.ID);
            var timerTask = Task.Delay(TimeSpan.FromMinutes(5));
            var completedTask = await Task.WhenAny(waitTask, timerTask);
            if (completedTask == waitTask)
            {
                var waitResult = await waitTask;
                if (waitResult.StatusCode != 0)
                {
                    logger.LogError("Container for building web application failed with status code: {StatusCode}",
                        waitResult.StatusCode);
                    throw new BadRequestException(
                        $"Building web application failed with status code: {waitResult.StatusCode}");
                }
                logger.LogInformation("Container for building web application completed successfully.");
            }
            else
            {
                logger.LogError("Building web application timed out after 5 minutes.");
                throw new BadRequestException("Building web application timed out after 5 minutes.");
            }
        }
        finally
        {
            try
            {
                await client.Containers.RemoveContainerAsync(
                    builderContainerReference.ID,
                    new ContainerRemoveParameters { Force = true });
                logger.LogInformation("Removed container for building web application: {ContainerId}", builderContainerReference.ID);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to remove container {ContainerId} after building web application.",
                    builderContainerReference.ID);
            }
        }
    }

    private async Task<string> GetBuilderDockerImage()
    {
        var builderDockerfileContext = GetWebApplicationBuilderDockerfileName();
        await client.Images.BuildImageFromDockerfileAsync(
            new ImageBuildParameters
            {
                Tags = [DockerNamingHelper.WebApplicationBuilderImageName],
                Dockerfile = "WebApplicationBuilder.Dockerfile",
            },
            builderDockerfileContext,
            null,
            new Dictionary<string, string>(),
            new Progress<JSONMessage>());
        builderDockerfileContext.Close();
        return DockerNamingHelper.WebApplicationBuilderImageName;
    }
    private Stream GetWebApplicationBuilderDockerfileName()
    {
        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
            throw new InvalidOperationException("Could not determine the current directory.");
        var dockerFile = Path.Combine(
            currentDirectory,
            "Containerization/Clients/WebApplication/Dockerfiles/WebApplicationBuilder.Dockerfile");

        var tarball = new MemoryStream();
        using var archive = new TarOutputStream(tarball, Encoding.UTF8)
        {
            IsStreamOwner = false
        };
        var entry = TarEntry.CreateTarEntry(dockerFile);
        using var fileStream = File.OpenRead(dockerFile);
        entry.Size = fileStream.Length;
        entry.Name = Path.GetFileName(dockerFile);
        archive.PutNextEntry(entry);
        var localBuffer = new byte[32 * 1024];
        while (true)
        {
            var numRead = fileStream.Read(localBuffer, 0, localBuffer.Length);
            if (numRead <= 0)
            {
                break;
            }

            archive.Write(localBuffer, 0, numRead);
        }
        archive.CloseEntry();
        archive.Close();
        tarball.Position = 0;
        return tarball;
    }
}
