using MediatR;
using System.Text.Json.Serialization;

namespace Kjkardum.CloudyBack.Application.UseCases.Kafka.Commands.ProduceTopicMessage;

public class ProduceKafkaTopicMessageCommand: IRequest
{
    [JsonIgnore] public Guid ServerId { get; set; }
    [JsonIgnore] public string Topic { get; set; } = string.Empty;
    public string Key { get; set; }
    public string Value { get; set; }
}
