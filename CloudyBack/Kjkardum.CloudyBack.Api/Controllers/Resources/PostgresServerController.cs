using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetPaginated;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.CreateDatabase;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.CreateServer;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.DeleteServer;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.RunDatabaseQuery;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.ServerContainerAction;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Queries.GetById;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Queries.GetDatabaseById;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Queries.GetPaginatedDatabases;
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
    [HttpGet("database")]
    public async Task<ActionResult<PaginatedResponse<PostgresDatabaseResourceDto>>> GetAllDatabases(
        [FromQuery] PaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetPaginatedPostgresDatabaseResourceQuery(request), cancellationToken);
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

    [HttpGet("database/{databaseId:guid}")]
    public async Task<ActionResult<PostgresDatabaseResourceDto>> GetDatabaseById(
        Guid databaseId,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetSinglePostgresDatabaseResourceQuery { Id = databaseId },
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("/database/{databaseId:guid}/runQuery")]
    public async Task<ActionResult<PostgresQueryResultDto>> RunDatabaseQuery(
        Guid databaseId,
        RunPostgresDatabaseQueryCommand command,
        CancellationToken cancellationToken = default)
    {
        command.DatabaseId = databaseId;
        var result = await mediator.Send(command, cancellationToken);
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

    [HttpPost("{serverId:guid}/database")]
    public async Task<ActionResult<PostgresDatabaseSimpleResourceDto>> CreateDatabase(
        Guid serverId,
        [FromBody] CreatePostgresDatabaseCommand request,
        CancellationToken cancellationToken = default)
    {
        request.ServerId = serverId;
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{serverId:guid}/containerAction")]
    public async Task<IActionResult> ExecuteContainerAction(Guid serverId, [FromQuery] string actionId)
    {
        await mediator.Send(new PostgresServerContainerActionCommand { Id = serverId, ActionId = actionId });
        return Ok();
    }

    [HttpDelete("{serverId:guid}")]
    public async Task<IActionResult> Delete(Guid serverId, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new DeletePostgresServerResourceCommand { Id = serverId }, cancellationToken);
        return NoContent();
    }
}
