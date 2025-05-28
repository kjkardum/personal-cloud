using CloudyPoC;
using Docker.DotNet;
using Docker.DotNet.Models;

var dockerClient = new DockerClientConfiguration().CreateClient();
const string imageName = "postgres";
const string containerName = "some-postgres";
const string volumeName = "dummydata";
const string userName = "postgres";

var appBuilder = WebApplication.CreateBuilder(args);
appBuilder.Services.AddEndpointsApiExplorer();
appBuilder.Services.AddSwaggerGen();
var app = appBuilder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapGet("/postgres/start", async () =>
{
    
    await dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
    {
        FromImage = imageName,
        Tag = "latest"
    }, null, new Progress<JSONMessage>());
    await dockerClient.Volumes.CreateAsync(new VolumesCreateParameters { Name = "dummydata" });
    await dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
    {
        Name = containerName,
        Image = imageName,
        HostConfig = new HostConfig
        { Binds = new List<string> { "dummydata:/var/lib/postgresql/data", } },
        ExposedPorts = new Dictionary<string, EmptyStruct>
        {
            { "5432", default }
        },
        Env = new List<string> { "POSTGRES_PASSWORD=postgres" },
    });
    await dockerClient.Containers.StartContainerAsync(containerName, new ContainerStartParameters());
});

app.MapGet("/postgres/cleanDelete", async () =>
{
    try {await dockerClient.Containers.StopContainerAsync(containerName, new ContainerStopParameters());} catch {}
    try {await dockerClient.Containers.RemoveContainerAsync(containerName, new ContainerRemoveParameters());} catch {}
    try {await dockerClient.Volumes.RemoveAsync(volumeName);} catch {}
    try {await dockerClient.Images.DeleteImageAsync(imageName, new ImageDeleteParameters());} catch {}
});
app.MapGet("/postgres/run-create-dummytable", async () =>
{
    var container = await dockerClient.Containers.InspectContainerAsync(containerName);
    var exec = await dockerClient.Exec.ExecCreateContainerAsync(container.ID, new ContainerExecCreateParameters
    {
        AttachStdin = true,
        AttachStdout = true,
        AttachStderr = true,
        Cmd = new List<string> { "psql", "-U", userName, "-P", "pager=off", "-P", "format=csv", "-c", "CREATE TABLE cars (brand VARCHAR(255), model VARCHAR(255),  year INT);" },
        Tty = true,
        Detach = false
    });
    var execStart = await dockerClient.Exec.StartAndAttachContainerExecAsync(exec.ID, true);
    var (stdOut, stdErr) = await execStart.ReadOutputToEndAsync(default);
    var entries = stdOut.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
        .Select(line => line.Split(",") /*Needs better splits*/).ToList();
    var dict = entries.Select(entry => entry.Zip(entries.First(), (value, header) => (header, value))
        .ToDictionary(pair => pair.header, pair => pair.value)).Skip(1);
    return dict;
});

app.MapGet("/postgres/run-select-pgtables", async () =>
{
    var container = await dockerClient.Containers.InspectContainerAsync(containerName);
    var exec = await dockerClient.Exec.ExecCreateContainerAsync(container.ID, new ContainerExecCreateParameters
    {
        AttachStdin = true,
        AttachStdout = true,
        AttachStderr = true,
        Cmd = new List<string> { "psql", "-U", userName, "-P", "pager=off", "-P", "format=csv", "-c", "SELECT * FROM pg_catalog.pg_tables;" },
        Tty = true,
        Detach = false
    });
    var execStart = await dockerClient.Exec.StartAndAttachContainerExecAsync(exec.ID, true);
    var (stdOut, stdErr) = await execStart.ReadOutputToEndAsync(default);
    var entries = stdOut.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
        .Select(line => line.Split(",") /*Needs better splits*/).ToList();
    var dict = entries.Select(entry => entry.Zip(entries.First(), (value, header) => (header, value))
        .ToDictionary(pair => pair.header, pair => pair.value)).Skip(1);
    return dict;
});

app.MapGet("/postgres/read-logs", async () =>
{
    var logs = await dockerClient.Containers.GetContainerLogsAsync(containerName, new ContainerLogsParameters
    {
        ShowStdout = true,
        ShowStderr = true,
        Timestamps = true
    });
    using var reader = new StreamReader(logs);
    return await reader.ReadToEndAsync();
});

app.MapGet("/network/next-subnet", async () =>
{
    var allocator = new DockerSubnetAllocator(dockerClient);
    var nextSubnet = await allocator.GetNextAvailableSubnet();
    return nextSubnet?.ToString() ?? "No available subnet found";
});

app.Run();