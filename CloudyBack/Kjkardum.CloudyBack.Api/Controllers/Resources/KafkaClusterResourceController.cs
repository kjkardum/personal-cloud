using Kjkardum.CloudyBack.Application.UseCases.Kafka.Commands.Create;
using Kjkardum.CloudyBack.Application.UseCases.Kafka.Dtos;
using Kjkardum.CloudyBack.Application.UseCases.Kafka.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kjkardum.CloudyBack.Api.Controllers.Resources;

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

    [HttpPost]
    public async Task<ActionResult<KafkaClusterResourceDto>> Create(
        CreateKafkaClusterCommand request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }
}
