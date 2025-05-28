namespace Kjkardum.CloudyBack.Application.Clients;

public interface IKafkaClient
{
    Task CreateClusterAsync(Guid id, string clusterName, string saUsername, string saPassword, int serverPort);
}
