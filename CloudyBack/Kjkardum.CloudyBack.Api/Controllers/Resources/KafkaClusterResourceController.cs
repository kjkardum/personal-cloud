using Kjkardum.CloudyBack.Application.UseCases.Kafka.Commands.ClusterContainerAction;
using Kjkardum.CloudyBack.Application.UseCases.Kafka.Commands.Create;
using Kjkardum.CloudyBack.Application.UseCases.Kafka.Commands.CreateTopic;
using Kjkardum.CloudyBack.Application.UseCases.Kafka.Commands.Delete;
using Kjkardum.CloudyBack.Application.UseCases.Kafka.Commands.ProduceTopicMessage;
using Kjkardum.CloudyBack.Application.UseCases.Kafka.Dtos;
using Kjkardum.CloudyBack.Application.UseCases.Kafka.Queries.ConsumeMessages;
using Kjkardum.CloudyBack.Application.UseCases.Kafka.Queries.GetById;
using Kjkardum.CloudyBack.Application.UseCases.Kafka.Queries.GetTopics;
using Kjkardum.CloudyBack.Infrastructure.Containerization.Clients.Kafka;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Kjkardum.CloudyBack.Api.Controllers.Resources;

[Authorize]
[ApiController]
[Route("api/resource/[controller]")]
public class KafkaClusterResourceController(IMediator mediator): ControllerBase
{
    [HttpGet("{serverId:guid}")]
    public async Task<ActionResult<KafkaClusterResourceDto>> GetById(
        Guid serverId,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetKafkaClusterByIdQuery { Id = serverId }, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{serverId:guid}/topics")]
    public async Task<ActionResult<List<KafkaTopicDto>>> GetTopics(
        Guid serverId,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetKafkaTopicsByClusterIdQuery { Id = serverId }, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{serverId:guid}/topics")]
    public async Task<IActionResult> CreateTopic(
        Guid serverId,
        CreateKafkaTopicCommand command,
        CancellationToken cancellationToken = default)
    {
        command.ServerId = serverId;
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("{serverId:guid}/topics/{topicId}")]
    public async Task<IActionResult> ProduceMessageToTopic(
        Guid serverId,
        string topicId,
        ProduceKafkaTopicMessageCommand command,
        CancellationToken cancellationToken = default)
    {
        command.ServerId = serverId;
        command.Topic = topicId;
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpGet("{serverId:guid}/topics/{topicId}/consumeLive")]
    public async IAsyncEnumerable<string> ConsumeLive(
        Guid serverId,
        string topicId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var messages = mediator.CreateStream(
            new KafkaConsumeMessagesQuery { Id = serverId, Topic = topicId},
            cancellationToken);
        await foreach (var message in messages)
        {
            yield return message;
        }
    }

    [HttpPost]
    public async Task<ActionResult<KafkaClusterResourceDto>> Create(
        CreateKafkaClusterCommand request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{serverId:guid}")]
    public async Task<IActionResult> Delete(
        Guid serverId,
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new DeleteKafkaClusterCommand { Id = serverId }, cancellationToken);
        return NoContent();
    }


    [HttpPost("{serverId:guid}/containerAction")]
    public async Task<IActionResult> ExecuteContainerAction(Guid serverId, [FromQuery] string actionId)
    {
        await mediator.Send(new KafkaClusterContainerActionCommand { Id = serverId, ActionId = actionId });
        return Ok();
    }
}
