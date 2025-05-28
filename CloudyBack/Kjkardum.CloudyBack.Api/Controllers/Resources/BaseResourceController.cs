using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetAuditLog;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetContainer;
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

    [HttpGet("{resourceId:guid}/container")]
    public async Task<ActionResult<ContainerDto>> GetContainer(Guid resourceId)
    {
        var result = await mediator.Send(new GetContainerBaseResourceQuery { Id = resourceId });
        return Ok(result);
    }

    [HttpGet("{resourceId:guid}/audit-log")]
    public async Task<ActionResult<PaginatedResponse<AuditLogEntry>>> GetAuditLog(
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
