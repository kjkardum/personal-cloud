using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetPaginated;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.CreateDatabase;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.CreateServer;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kjkardum.CloudyBack.Api.Controllers.Resources;

[ApiController]
[Route("api/resource/[controller]")]
public class PostgresServerResourceController(IMediator mediator): ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<PostgresServerResourceDto>>> GetAll(
        [FromQuery] PaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetPaginatedPostgresServerResourceQuery(request), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{serverId:guid}")]
    public async Task<ActionResult<PostgresServerResourceDto>> GetById(
        Guid serverId,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetSinglePostgresServerResourceQuery { Id = serverId }, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<PostgresServerResourceDto>> Create(
        CreatePostgresServerCommand request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{serverId}/database")]
    public async Task<ActionResult<PostgresDatabaseResourceDto>> CreateDatabase(
        Guid serverId,
        [FromBody] CreatePostgresDatabaseCommand request,
        CancellationToken cancellationToken = default)
    {
        request.ServerId = serverId;
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }
}
