using Docker.DotNet;
using Docker.DotNet.Models;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.UseCases.Kafka.Dtos;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.Kafka.Helpers;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Helpers;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text;

namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.Kafka;

public class KafkaClient(DockerClient client, ILogger<KafkaClient> logger) : IKafkaClient
{
    private const string ImageName = "apache/kafka-native:3.9.1";
    private const string ScriptRunnerImageName = "confluentinc/cp-kafkacat";
    private const string KafkaExporterImageName = "otel/opentelemetry-collector-contrib";

    public async Task CreateClusterAsync(
        Guid id,
        string clusterName)
    {
        await CreateCommonVolume(id);
        await CreateCommonNetwork(id);
        await CreateCombinedContainer(id);
        await CreateSidecarTelemetryCollector(id, clusterName);
    }

    public async Task StartServerAsync(Guid id)
    {
        var controllerName = DockerNamingHelper.GetContainerName(id);
        client.Containers.StartContainerAsync(
            controllerName,
            new ContainerStartParameters());
        logger.LogInformation("Kafka server started");
    }

    public async Task StopServerAsync(Guid id)
    {
        var controllerName = DockerNamingHelper.GetContainerName(id);
        await client.Containers.StopContainerAsync(
            controllerName,
            new ContainerStopParameters());
        logger.LogInformation("Kafka server stopped");
    }

    public async Task RestartServerAsync(Guid id)
    {
        var controllerName = DockerNamingHelper.GetContainerName(id);
        await client.Containers.RestartContainerAsync(
            controllerName,
            new ContainerRestartParameters());
        logger.LogInformation("Kafka server restarted");
    }


    private async Task CreateCommonVolume(Guid id)
    {
        await client.Volumes.CreateAsync(new VolumesCreateParameters { Name = DockerNamingHelper.GetVolumeName(id) });
        logger.LogInformation("Kafka volume created");
    }

    private async Task CreateCommonNetwork(Guid id)
    {
        var networkName = DockerNamingHelper.GetNetworkName(id);
        var networks = await client.Networks.ListNetworksAsync();
        if (networks.Any(n => n.Name == networkName))
        {
            logger.LogInformation("Kafka network already exists");
            return;
        }

        await client.Networks.CreateNetworkAsync(new NetworksCreateParameters
        {
            Name = networkName, Driver = "bridge"
        });
        logger.LogInformation("Kafka network created");
    }

    private async Task CreateCombinedContainer(Guid id)
    {
        await client.Images.CreateImageAsync(
            new ImagesCreateParameters { FromImage = ImageName, Tag = "3.9.1" },
            null,
            new Progress<JSONMessage>());
        logger.LogInformation("Kafka image pulled for controller");

        var controllerName = DockerNamingHelper.GetContainerName(id);
        await client.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Name = controllerName,
            Image = ImageName,
            NetworkingConfig =
                new NetworkingConfig()
                {
                    EndpointsConfig = new Dictionary<string, EndpointSettings>
                    {
                        { DockerNamingHelper.GetNetworkName(id), new EndpointSettings() }
                    }
                },
            HostConfig =
                new HostConfig
                {
                    Binds = new List<string>
                    {
                        $"{DockerNamingHelper.GetVolumeName(id)}_controller:/var/lib/kafka/data",
                    }
                },
            Env = new List<string>
            {
                "KAFKA_HEAP_OPTS=-Xmx512M -Xms512M",
                "KAFKA_LISTENERS=CONTROLLER://localhost:9091,DOCKER://0.0.0.0:9092",
                $"KAFKA_ADVERTISED_LISTENERS=DOCKER://{DockerNamingHelper.GetContainerName(id)}:9092",
                "KAFKA_LISTENER_SECURITY_PROTOCOL_MAP=CONTROLLER:PLAINTEXT,DOCKER:PLAINTEXT",
                "KAFKA_NODE_ID=1",
                "KAFKA_PROCESS_ROLES=broker,controller",
                "KAFKA_CONTROLLER_LISTENER_NAMES=CONTROLLER",
                "KAFKA_CONTROLLER_QUORUM_VOTERS=1@localhost:9091",
                "KAFKA_INTER_BROKER_LISTENER_NAME=DOCKER",
                "KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR=1"
            }
        });
        logger.LogInformation("Kafka controller container created");

        await client.Containers.StartContainerAsync(
            controllerName,
            new ContainerStartParameters());
        logger.LogInformation("Kafka controller container started");
    }

    private async Task CreateSidecarTelemetryCollector(Guid id, string clusterName)
    {
        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                               throw new InvalidOperationException("Could not determine the current directory.");
        var collectorYamlTemplate = Path.Combine(
            currentDirectory,
            "Containerization/Clients/Kafka/FileTemplates/collector.yml");

        await client.Images.CreateImageAsync(
            new ImagesCreateParameters { FromImage = KafkaExporterImageName, Tag = "latest" },
            null,
            new Progress<JSONMessage>());
        logger.LogInformation("Kafka telemetry collector image pulled");

        var brokerName = DockerNamingHelper.GetContainerName(id);

        await client.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Name = DockerNamingHelper.GetSidecarTelemetryName(id),
            Image = KafkaExporterImageName,
            NetworkingConfig =
                new NetworkingConfig()
                {
                    EndpointsConfig = new Dictionary<string, EndpointSettings>()
                    {
                        { DockerNamingHelper.GetNetworkName(id), new EndpointSettings() },
                        { DockerNamingHelper.ObservabilityNetworkName, new EndpointSettings() }
                    }
                },
            HostConfig =
                new HostConfig { Binds = new List<string> { $"{collectorYamlTemplate}:/conf/collector.yml" } },
            Env = new List<string>
            {
                $"KAFKA_BOOTSTRAP_SERVERS={brokerName}:9092",
                $"KAFKA_CLUSTER_NAME={clusterName}",
                $"OTLP_ENDPOINT=http://{DockerNamingHelper.LokiContainerName}:3100/otlp"
            },
            Cmd = new List<string> { "--config=/conf/collector.yml" }
        });
        logger.LogInformation("Kafka telemetry collector container created");

        await client.Containers.StartContainerAsync(
            DockerNamingHelper.GetSidecarTelemetryName(id),
            new ContainerStartParameters());
        logger.LogInformation("Kafka telemetry collector container started");
    }

    public async Task CreateTopicAsync(Guid id, string topicName)
    {
        var args = new List<string>
        {
            "-X", "broker.version.fallback=3.9.1",
            "-L",
            "-t", topicName,
            "-P",
        };

        logger.LogInformation("Creating Kafka topic {TopicName}", topicName);
        var (stdOut, _) = await RunKafkacatContainerAsync(id, args, new List<string>(), false);
        logger.LogInformation("Topic creation response:\n{StdOut}", stdOut);
    }
    public async Task<List<KafkaTopicDto>> GetTopicsAsync(Guid id)
    {
        var args = new List<string>
        {
            "-L"
        };

        var (stdOut, _) = await RunKafkacatContainerAsync(id, args, new List<string>(), true);
        logger.LogInformation("Raw topics metadata:\n{StdOut}", stdOut);

        // Parse output using your existing deserializer
        var result = KafkaTopicDeserializer.ParseKafkaTopicsOutput(stdOut);
        logger.LogInformation("Parsed topics count: {Count}", result.Count);

        return result;
    }

    public async Task ProduceMessageAsync(
        Guid id,
        string topicName,
        string message,
        string? key = null)
    {
        var args = new List<string>
        {
            "-X",
            "broker.version.fallback=3.9.1",
            "-t",
            topicName,
            "-K:", // Use ':' as key-value separator
            "-D",
            "«",
            "-P", // Produce mode
            "-l",
            "/data/msg.txt"
        };

        logger.LogInformation("Producing message to Kafka topic {TopicName}", topicName);
        var (stdOut, stdErr) = await RunKafkacatContainerAsync(
            id,
            args,
            [
                $"{await SaveMessageToFileAsync($"{key ?? string.Empty}:" + message)}:/data/msg.txt"
            ],
            true);

        if (!string.IsNullOrEmpty(stdErr))
        {
            logger.LogError("Error producing message: {StdErr}", stdErr);
            throw new Exception($"Failed to produce message: {stdErr}");
        }

        logger.LogInformation("Message produced successfully: {StdOut}", stdOut);
    }

    public async IAsyncEnumerable<string> StreamTopicMessagesAsync(
        Guid id,
        string topicName,
        [System.Runtime.CompilerServices.EnumeratorCancellation]
        CancellationToken cancellationToken = default)
    {
        var broker = DockerNamingHelper.GetContainerName(id) + ":9092";
        var format = "Key=%k\nPartition=%p\nOffset=%o\nValue=%s\n--=--\n";

        // Pull kafkacat image if necessary
        await client.Images.CreateImageAsync(
            new ImagesCreateParameters { FromImage = "confluentinc/cp-kafkacat", Tag = "latest" },
            null,
            new Progress<JSONMessage>()
        );
        logger.LogInformation("Pulled kafkacat image");

        // Create a temporary container for kafkacat
        var create = await client.Containers.CreateContainerAsync(
            new CreateContainerParameters
            {
                Image = "confluentinc/cp-kafkacat:latest",
                Name = DockerNamingHelper.GetScalableContainerName(id, 0) + "-kcat-" + Guid.NewGuid().ToString("N"),
                NetworkingConfig =
                    new NetworkingConfig()
                    {
                        EndpointsConfig = new Dictionary<string, EndpointSettings>()
                        {
                            { DockerNamingHelper.GetNetworkName(id), new EndpointSettings() }
                        }
                    },
                Cmd = new List<string>
                    {
                        "kafkacat",
                        "-b",
                        broker,
                        "-C",
                        "-K:",
                        "-f",
                        format,
                        "-t",
                        topicName
                    }
                    .Where(arg => arg is not null)
                    .ToList(),
                Tty = true,
                AttachStdout = true,
                AttachStderr = true
            },
            cancellationToken);

        var containerId = create.ID;
        await client.Containers.StartContainerAsync(containerId, new ContainerStartParameters(), cancellationToken);
        logger.LogInformation("Started kafkacat consumer container {Container}", containerId);

        // Attach and read logs
        var stream = await client.Containers.GetContainerLogsAsync(
            containerId,
            new ContainerLogsParameters { ShowStdout = true, ShowStderr = true, Follow = true, Tail = "all" },
            cancellationToken);

        using var reader = new StreamReader(stream);
        var buffer = new StringBuilder();
        string? line;
        try
        {
            while (!cancellationToken.IsCancellationRequested
                   && (line = await reader.ReadLineAsync(cancellationToken)) != null)
            {
                buffer.AppendLine(line);
                if (line == "--=--")
                {
                    // Emit full message block
                    yield return buffer.ToString().TrimEnd();
                    buffer.Clear();
                }
            }
        }
        finally
        {
            // Cleanup
            await client.Containers.StopContainerAsync(containerId, new ContainerStopParameters());
            await client.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters { Force = true });
            logger.LogInformation("Stopped and removed kafkacat container {Container}", containerId);
        }
    }

    private async Task<(string StdOut, string StdErr)> RunKafkacatContainerAsync(
        Guid id,
        IList<string> args,
        IList<string> mountList,
        bool stdout,
        CancellationToken cancellationToken = default)
    {
        // Pull image if necessary
        await client.Images.CreateImageAsync(
            new ImagesCreateParameters { FromImage = ScriptRunnerImageName, Tag = "latest" },
            null,
            new Progress<JSONMessage>());

        var broker = DockerNamingHelper.GetContainerName(id) + ":9092";
        var containerName = DockerNamingHelper.GetScalableContainerName(id, 0)
            + "-kcat-"
            + Guid.NewGuid().ToString("N");

        var create = await client.Containers.CreateContainerAsync(
            new CreateContainerParameters
            {
                Image = ScriptRunnerImageName,
                Name = containerName,
                NetworkingConfig = new NetworkingConfig
                {
                    EndpointsConfig = new Dictionary<string, EndpointSettings>
                    {
                        { DockerNamingHelper.GetNetworkName(id), new EndpointSettings() }
                    }
                },
                HostConfig = new HostConfig
                {
                    Binds = mountList
                },
                Cmd = args.Prepend(broker).Prepend("-b").Prepend("kafkacat").ToList(),
                Tty = true,
                AttachStdout = true,
                AttachStderr = true
            },
            cancellationToken);

        await client.Containers.StartContainerAsync(create.ID, new ContainerStartParameters(), cancellationToken);

        var stdOut = new StringBuilder();
        if (stdout)
        {
            var logs = await client.Containers.GetContainerLogsAsync(
                create.ID,
                new ContainerLogsParameters { ShowStdout = true, ShowStderr = true, Follow = true },
                cancellationToken);

            using var reader = new StreamReader(logs);
            string? line;
            while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
            {
                stdOut.AppendLine(line);
            }
        }

        await client.Containers.StopContainerAsync(create.ID, new ContainerStopParameters());
        await client.Containers.RemoveContainerAsync(
            create.ID,
            new ContainerRemoveParameters { Force = true });

        logger.LogInformation("Executed kafkacat container {ContainerName}", containerName);
        return (stdOut.ToString(), string.Empty); // kafkacat logs everything to stdout by default
    }
    public static async Task<string> SaveMessageToFileAsync(string message)
    {
        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
            throw new InvalidOperationException("Could not determine the current directory.");
        var basePath = Path.Combine(
            currentDirectory,
            "Containerization/Clients/Kafka/FileTemplates/messages");
        Directory.CreateDirectory(basePath);

        // Create a subdirectory with a random GUID

        // Create the file path
        var filename = $"kafkamessage{Guid.NewGuid():N}.txt";
        var filePath = Path.Combine(basePath, filename);

        // Write the message to the file
        await File.WriteAllTextAsync(filePath, message);

        return filePath;
    }
}
