using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Kafka.Queries.ConsumeMessages;

public class KafkaConsumeMessagesQuery: IStreamRequest<string>
{
    public Guid Id { get; set; }
    public string Topic { get; set; }
}
