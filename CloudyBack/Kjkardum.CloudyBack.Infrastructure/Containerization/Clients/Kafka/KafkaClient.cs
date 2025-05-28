using Docker.DotNet;
using Docker.DotNet.Models;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Helpers;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.Kafka;

public class KafkaClient(DockerClient client, ILogger<KafkaClient> logger) : IKafkaClient
{
    private const string ImageName = "apache/kafka:3.9.1";
    private const string KafkaExporterImageName = "otel/opentelemetry-collector-contrib";
    private const int KafkaPort = 9092;
    private const int ControllerPort = 9091;
    private const int DockerPort = 9093;

    public async Task CreateClusterAsync(
        Guid id,
        string clusterName,
        string saUsername,
        string saPassword,
        int serverPort)
    {
        await CreateCommonVolume(id);
        await CreateCommonNetwork(id);
        await CreateServerContainer(id, saUsername, saPassword, serverPort);
        await ExecuteServerSuperUserPassword(id, saUsername, saPassword);
        await CreateSidecarTelemetryCollector(id, clusterName, saUsername, saPassword);
    }

    private async Task CreateCommonVolume(Guid id)
    {
        await client.Volumes.CreateAsync(new VolumesCreateParameters
            {
                Name = DockerNamingHelper.GetVolumeName(id)
            });
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
                Name = networkName,
                Driver = "bridge"
            });
        logger.LogInformation("Kafka network created");
    }

    private async Task CreateServerContainer(Guid id, string saUsername, string saPassword, int serverPort)
    {
        await client.Images.CreateImageAsync(
            new ImagesCreateParameters { FromImage = ImageName, Tag = "3.9.1" },
            null,
            new Progress<JSONMessage>());
        logger.LogInformation("Kafka image pulled");

        await client.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Name = DockerNamingHelper.GetContainerName(id),
                Image = ImageName,
                NetworkingConfig = new NetworkingConfig()
                {
                    EndpointsConfig = new Dictionary<string, EndpointSettings>()
                    {
                        { DockerNamingHelper.GetNetworkName(id), new EndpointSettings() }
                    }
                },
                HostConfig = new HostConfig
                {
                    Binds = new List<string>
                    {
                        $"{DockerNamingHelper.GetVolumeName(id)}:/var/lib/kafka/data",
                        $"{GetJaasConfigPath(id, saPassword)}:/opt/kafka/config/kafka_server_jaas.conf"
                    },
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        { $"{KafkaPort}/tcp", new List<PortBinding> { new() { HostPort = $"{serverPort}" } } }
                    }
                },
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    { $"{KafkaPort}/tcp", default },
                    { $"{ControllerPort}/tcp", default }
                },
                Env = new List<string>
                {
                    "KAFKA_NODE_ID=1",
                    "KAFKA_PROCESS_ROLES=broker,controller",
                    "KAFKA_CONTROLLER_LISTENER_NAMES=CONTROLLER",
                    $"KAFKA_LISTENERS=CONTROLLER://0.0.0.0:{ControllerPort},BROKER://0.0.0.0:{KafkaPort},DOCKER://0.0.0.0:{DockerPort}",
                    $"KAFKA_ADVERTISED_LISTENERS=BROKER://localhost:{KafkaPort},DOCKER://:{DockerNamingHelper.GetContainerName(id)}:{DockerPort}",
                    "KAFKA_LISTENER_SECURITY_PROTOCOL_MAP=CONTROLLER:SASL_PLAINTEXT,BROKER:SASL_PLAINTEXT,DOCKER:SASL_PLAINTEXT",
                    $"KAFKA_CONTROLLER_QUORUM_VOTERS=1@localhost:{ControllerPort}",
                    $"KAFKA_SUPER_USERS=User:{saUsername},User:broker",
                    "KAFKA_INTER_BROKER_LISTENER_NAME=DOCKER",
                    "KAFKA_SASL_MECHANISM_INTER_BROKER_PROTOCOL=PLAIN",
                    "KAFKA_SASL_MECHANISM_CONTROLLER_PROTOCOL=PLAIN",
                    "KAFKA_SASL_ENABLED_MECHANISMS=PLAIN",
                    "KAFKA_OPTS=-Djava.security.auth.login.config=/opt/kafka/config/kafka_server_jaas.conf",
                    "KAFKA_OFFSET_TOPIC_REPLICATION_FACTOR=1",
                    "KAFKA_AUTHORIZER_CLASS_NAME=org.apache.kafka.metadata.authorizer.StandardAuthorizer",
                    "KAFKA_ALLOW_EVERYONE_IF_NO_ACL_FOUND=false",
                    "KAFKA_LOG_DIRS=/var/lib/kafka/data"
                }
            });
        logger.LogInformation("Kafka container created");

        await client.Containers.StartContainerAsync(
            DockerNamingHelper.GetContainerName(id),
            new ContainerStartParameters());
        logger.LogInformation("Kafka container started");
    }

    private string GetJaasConfigPath(Guid id, string saPassword)
    {
        var jaasConfig = $$"""
                         KafkaServer {
                             org.apache.kafka.common.security.plain.PlainLoginModule required
                             username="broker"
                             password="{{saPassword}}"
                             user_broker="{{saPassword}}";
                             user_admin="{{saPassword}}";
                         };
                         """;

        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
            throw new InvalidOperationException("Could not determine the current directory.");
        var kafkaFileTemplateFolder = Path.Combine(
            currentDirectory,
            "Containerization/Clients/Kafka/FileTemplates");
        var configDir = Path.Combine(kafkaFileTemplateFolder, id.ToString());
        Directory.CreateDirectory(configDir);
        var jaasPath = Path.Combine(configDir, "kafka_server_jaas.conf");
        File.WriteAllText(jaasPath, jaasConfig);
        return jaasPath;
    }

    private async Task ExecuteServerSuperUserPassword(Guid id, string saUsername, string saPassword)
    {
        var container = await client.Containers.InspectContainerAsync(DockerNamingHelper.GetContainerName(id));
        logger.LogInformation("Kafka container found for user setup");

        var createUserCommand = $"SCRAM-SHA-512=[password={saPassword}]";

        var exec = await client.Exec.ExecCreateContainerAsync(
            container.ID,
            new ContainerExecCreateParameters
            {
                AttachStdin = true,
                AttachStdout = true,
                AttachStderr = true,
                Cmd = new List<string>
                {
                    "/opt/kafka/bin/kafka-configs.sh",
                    "--bootstrap-server",
                    $"localhost:{KafkaPort}",
                    "--alter",
                    "--add-config",
                    createUserCommand,
                    "--entity-type",
                    "users",
                    "--entity-name",
                    saUsername
                },
                Tty = true,
                Detach = false
            });

        logger.LogInformation("Kafka super user exec created");
        var execStart = await client.Exec.StartAndAttachContainerExecAsync(exec.ID, true);
        var (stdOut, stdErr) = await execStart.ReadOutputToEndAsync(default);
        logger.LogInformation("Kafka super user created. Response: {StdOut}", stdOut);
    }

    private async Task CreateSidecarTelemetryCollector(Guid id, string clusterName, string saUsername, string saPassword)
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

        await client.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Name = DockerNamingHelper.GetSidecarTelemetryName(id),
                Image = KafkaExporterImageName,
                NetworkingConfig = new NetworkingConfig()
                {
                    EndpointsConfig = new Dictionary<string, EndpointSettings>()
                    {
                        { DockerNamingHelper.GetNetworkName(id), new EndpointSettings() },
                        { DockerNamingHelper.ObservabilityNetworkName, new EndpointSettings() }
                    }
                },
                HostConfig = new HostConfig
                {
                    Binds = new List<string> { $"{collectorYamlTemplate}:/conf/collector.yml" }
                },
                Env = new List<string>
                {
                    $"KAFKA_SASL_USERNAME={saUsername}",
                    $"KAFKA_SASL_PASSWORD={saPassword}",
                    $"KAFKA_BOOTSTRAP_SERVERS={DockerNamingHelper.GetContainerName(id)}:{KafkaPort}",
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
        var container = await client.Containers.InspectContainerAsync(DockerNamingHelper.GetContainerName(id));
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
                    $"localhost:{KafkaPort}",
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

    public async Task CreateUserAsync(Guid id, string username, string password)
    {
        var container = await client.Containers.InspectContainerAsync(DockerNamingHelper.GetContainerName(id));
        logger.LogInformation("Creating Kafka user: {Username}", username);

        var createUserCommand = $"SCRAM-SHA-512=[password={password}]";

        var exec = await client.Exec.ExecCreateContainerAsync(
            container.ID,
            new ContainerExecCreateParameters
            {
                AttachStdin = true,
                AttachStdout = true,
                AttachStderr = true,
                Cmd = new List<string>
                {
                    "/opt/kafka/bin/kafka-configs.sh",
                    "--bootstrap-server",
                    $"localhost:{KafkaPort}",
                    "--alter",
                    "--add-config",
                    createUserCommand,
                    "--entity-type",
                    "users",
                    "--entity-name",
                    username
                },
                Tty = true,
                Detach = false
            });

        var execStart = await client.Exec.StartAndAttachContainerExecAsync(exec.ID, true);
        var (stdOut, stdErr) = await execStart.ReadOutputToEndAsync(default);
        logger.LogInformation("User created. Response: {StdOut}", stdOut);
    }

    public async Task GrantTopicAccessAsync(Guid id, string username, string topicName, string accessType, string consumerGroup = null)
    {
        var container = await client.Containers.InspectContainerAsync(DockerNamingHelper.GetContainerName(id));
        logger.LogInformation("Granting {AccessType} access to user {Username} for topic {TopicName}", accessType, username, topicName);

        var operations = accessType.ToLower() switch
        {
            "read" => new[] { "Read", "Describe" },
            "write" => new[] { "Write", "Describe" },
            "readwrite" => new[] { "Read", "Write", "Describe" },
            _ => throw new ArgumentException($"Invalid access type: {accessType}")
        };

        foreach (var operation in operations)
        {
            var aclCommand = new List<string>
            {
                "/opt/kafka/bin/kafka-acls.sh",
                "--bootstrap-server",
                $"localhost:{KafkaPort}",
                "--add",
                "--allow-principal",
                $"User:{username}",
                "--operation",
                operation,
                "--topic",
                topicName
            };

            if (operation == "Read" && !string.IsNullOrEmpty(consumerGroup))
            {
                aclCommand.AddRange(new[] { "--group", consumerGroup });
            }

            var exec = await client.Exec.ExecCreateContainerAsync(
                container.ID,
                new ContainerExecCreateParameters
                {
                    AttachStdin = true,
                    AttachStdout = true,
                    AttachStderr = true,
                    Cmd = aclCommand,
                    Tty = true,
                    Detach = false
                });

            var execStart = await client.Exec.StartAndAttachContainerExecAsync(exec.ID, true);
            var (stdOut, stdErr) = await execStart.ReadOutputToEndAsync(default);
            logger.LogInformation("ACL {Operation} granted. Response: {StdOut}", operation, stdOut);
        }
    }
}
