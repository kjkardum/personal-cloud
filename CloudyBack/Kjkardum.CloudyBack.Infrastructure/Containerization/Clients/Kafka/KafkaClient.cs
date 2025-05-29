using Docker.DotNet;
using Docker.DotNet.Models;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.UseCases.Kafka.Dtos;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.Kafka.Helpers;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Helpers;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.Kafka;

public class KafkaClient(DockerClient client, ILogger<KafkaClient> logger) : IKafkaClient
{
    private const string ImageName = "apache/kafka:3.9.1";
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

    public async Task CreateTopicAsync(Guid id, string topicName, int partitions = 3, int replicationFactor = 1)
    {
        var brokerName = DockerNamingHelper.GetContainerName(id);
        var container = await client.Containers.InspectContainerAsync(brokerName);
        logger.LogInformation("Creating Kafka topic: {TopicName}", topicName);

        var exec = await client.Exec.ExecCreateContainerAsync(
            container.ID,
            new ContainerExecCreateParameters
            {
                AttachStdin = true,
                AttachStdout = true,
                AttachStderr = true,
                Cmd = new List<string>
                {
                    "/opt/kafka/bin/kafka-topics.sh",
                    "--create",
                    "--topic",
                    topicName,
                    "--bootstrap-server",
                    $"localhost:9092",
                    "--partitions",
                    partitions.ToString(),
                    "--replication-factor",
                    replicationFactor.ToString()
                },
                Tty = true,
                Detach = false
            });

        var execStart = await client.Exec.StartAndAttachContainerExecAsync(exec.ID, true);
        var (stdOut, stdErr) = await execStart.ReadOutputToEndAsync(default);
        logger.LogInformation("Topic created. Response: {StdOut}", stdOut);
    }

    public async Task<List<KafkaTopicDto>> GetTopicsAsync(Guid id)
    {
        var brokerName = DockerNamingHelper.GetContainerName(id);
        var container = await client.Containers.InspectContainerAsync(brokerName);
        logger.LogInformation("Retrieving Kafka topics");

        var exec = await client.Exec.ExecCreateContainerAsync(
            container.ID,
            new ContainerExecCreateParameters
            {
                AttachStdin = true,
                AttachStdout = true,
                AttachStderr = true,
                Cmd = new List<string>
                {
                    "/opt/kafka/bin/kafka-topics.sh", "--describe", "--bootstrap-server", "localhost:9092"
                },
                Tty = true,
                Detach = false
            });

        var execStart = await client.Exec.StartAndAttachContainerExecAsync(exec.ID, true);
        var (stdOut, stdErr) = await execStart.ReadOutputToEndAsync(default);
        var result = KafkaTopicDeserializer.ParseKafkaTopicsOutput(stdOut);
        logger.LogInformation("Topics retrieved. Response count: {StdOut}", result.Count);
        return result;
    }

    public async IAsyncEnumerable<string> StreamTopicMessagesAsync(
        Guid id,
        string topicName,
        bool fromBeginning = false,
        [System.Runtime.CompilerServices.EnumeratorCancellation]
        CancellationToken cancellationToken = default)
    {
        var brokerName = DockerNamingHelper.GetContainerName(id);
        var container = await client.Containers.InspectContainerAsync(brokerName, cancellationToken);
        logger.LogInformation("Streaming messages from Kafka topic: {TopicName}", topicName);
        var cmd = new List<string>
        {
            "/opt/kafka/bin/kafka-console-consumer.sh",
            "--topic",
            topicName,
            "--bootstrap-server",
            "localhost:9092"
        };
        if (fromBeginning)
        {
            cmd.Add("--from-beginning");
        }

        var exec = await client.Exec.ExecCreateContainerAsync(
            container.ID,
            new ContainerExecCreateParameters
            {
                AttachStdin = true,
                AttachStdout = true,
                AttachStderr = true,
                Cmd = cmd,
                Tty = false,
                Detach = false
            },
            cancellationToken);

        var execStart = await client.Exec.StartAndAttachContainerExecAsync(exec.ID, true, cancellationToken);
        logger.LogInformation("Kafka topic consumer started for topic: {TopicName}", topicName);
        await foreach (var p in ConsumerAsAsyncEnumerable(execStart, cancellationToken))
        {
            yield return p;
        }
    }

    private async IAsyncEnumerable<string> ConsumerAsAsyncEnumerable(
        MultiplexedStream execStart,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const int bufferSize = 4096; // Smaller buffer for more responsive line reading
        var buffer = new byte[bufferSize];
        var lineBuffer = new StringBuilder();
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await execStart.ReadOutputAsync(buffer, 0, buffer.Length, cancellationToken);
                if (result.EOF)
                {
                    break;
                }

                // Convert the bytes to string
                var chunk = Encoding.UTF8.GetString(buffer, 0, result.Count);

                // Process character by character to detect line endings immediately
                foreach (var c in chunk)
                {
                    if (c == '\n')
                    {
                        // Complete line found - yield it immediately
                        var line = lineBuffer.ToString().TrimEnd('\r');
                        lineBuffer.Clear();

                        if (!string.IsNullOrEmpty(line))
                        {
                            yield return line;
                        }
                    }
                    else
                    {
                        lineBuffer.Append(c);
                    }
                }
            }

            // Yield any remaining content in the buffer as the final line
            if (lineBuffer.Length <= 0)
            {
                yield break;
            }

            var finalLine = lineBuffer.ToString().TrimEnd('\r', '\n');
            if (!string.IsNullOrEmpty(finalLine))
            {
                yield return finalLine;
            }
        }
        finally
        {
            logger.LogInformation("Kafka topic consumer finished");
            execStart?.Dispose();
        }
    }
}
