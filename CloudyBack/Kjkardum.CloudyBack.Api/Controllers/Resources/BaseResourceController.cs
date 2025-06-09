using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Commands.ConfigureGrafana;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetAuditLog;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetContainer;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetDockerEnvironment;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetGrafanaConfiguration;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetPaginated;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.QueryLoki;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.QueryPrometheus;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kjkardum.CloudyBack.Api.Controllers.Resources;

[ApiController]
[Route("api/resource/[controller]")]
public class BaseResourceController(IMediator mediator): ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<BaseResourceDto>>> GetAll(
        [FromQuery] PaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetPaginatedBaseResourceQuery(request), cancellationToken);
        return Ok(result);
    }

    [HttpGet("dockerEnvironment")]
    public async Task<ActionResult<DockerEnvironment>> GetDockerEnvironment(
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetDockerEnvironmentQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("grafana")]
    public async Task<ActionResult<GrafanaConfigurationDto>> GetGrafanaConfiguration(
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetGrafanaConfigurationQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("grafana")]
    public async Task<IActionResult> UpdateGrafanaConfiguration(
        ConfigureGrafanaCommand command,
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpGet("{resourceId:guid}/container")]
    public async Task<ActionResult<ContainerDto>> GetContainer(Guid resourceId)
    {
        var result = await mediator.Send(new GetContainerBaseResourceQuery { Id = resourceId });
        return Ok(result);
    }

    [HttpGet("{resourceId:guid}/audit-log")]
    public async Task<ActionResult<PaginatedResponse<AuditLogDto>>> GetAuditLog(
        Guid resourceId,
        [FromQuery] PaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetPaginatedBaseResourceAuditLogQuery(resourceId, request),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("{resourceId:guid}/prometheus")]
    public async Task<ActionResult<PrometheusResultDto>> GetPrometheusMetrics(
        Guid resourceId,
        [FromBody] QueryPrometheusQuery query,
        CancellationToken cancellationToken = default)
    {
        query.ResourceId = resourceId;
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{resourceId:guid}/loki")]
    public async Task<ActionResult<PrometheusResultDto>> GetLokiMetrics(
        Guid resourceId,
        [FromBody] QueryLokiQuery query,
        CancellationToken cancellationToken = default)
    {
        query.ResourceId = resourceId;
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
