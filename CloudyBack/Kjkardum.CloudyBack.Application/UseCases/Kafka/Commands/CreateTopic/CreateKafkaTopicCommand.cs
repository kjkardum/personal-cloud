using MediatR;
using System.Text.Json.Serialization;

namespace Kjkardum.CloudyBack.Application.UseCases.Kafka.Commands.CreateTopic;

public class CreateKafkaTopicCommand: IRequest
{
    [JsonIgnore] public Guid ServerId { get; set; }
    public string TopicName { get; set; }
}
