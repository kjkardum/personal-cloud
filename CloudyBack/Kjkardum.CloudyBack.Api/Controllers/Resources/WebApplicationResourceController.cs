using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.WebApplication.Commands.Create;
using Kjkardum.CloudyBack.Application.UseCases.WebApplication.Commands.Delete;
using Kjkardum.CloudyBack.Application.UseCases.WebApplication.Commands.DeleteConfigItem;
using Kjkardum.CloudyBack.Application.UseCases.WebApplication.Commands.ModifyConfigItem;
using Kjkardum.CloudyBack.Application.UseCases.WebApplication.Commands.ServerContainerAction;
using Kjkardum.CloudyBack.Application.UseCases.WebApplication.Commands.UpdateDeploymentConfiguration;
using Kjkardum.CloudyBack.Application.UseCases.WebApplication.Dto;
using Kjkardum.CloudyBack.Application.UseCases.WebApplication.Queries.GetById;
using Kjkardum.CloudyBack.Application.UseCases.WebApplication.Queries.GetPaginated;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kjkardum.CloudyBack.Api.Controllers.Resources;

[ApiController]
[Route("api/resource/[controller]")]
public class WebApplicationResourceController(IMediator mediator): ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<WebApplicationResourceDto>>> GetAll(
        [FromQuery] PaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetPaginatedWebApplicationResourceQuery(request), cancellationToken);
        return Ok(result);
    }
    [HttpPost]
    public async Task<ActionResult<WebApplicationResourceDto>> CreateWebApplicationResource(
        CreateWebApplicationResourceCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WebApplicationResourceDto>> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetWebApplicationResourceByIdQuery { Id = id }, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}/deploymentConfiguration")]
    public async Task<IActionResult> UpdateWebApplicationDeploymentConfiguration(
        Guid id,
        UpdateWebApplicationDeploymentConfigurationCommand command,
        CancellationToken cancellationToken = default)
    {
        command.Id = id;
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
/*
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteWebApplicationResource(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new DeleteWebApplicationResourceCommand { Id = id }, cancellationToken);
        return NoContent();
    }
    */

    [HttpPost("{id:guid}/configuration")]
    public async Task<IActionResult> UpdateWebApplicationConfiguration(
        Guid id,
        ModifyWebApplicationConfigItemCommand command,
        CancellationToken cancellationToken = default)
    {
        command.WebApplicationId = id;
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}/configuration/{configurationKey}")]
    public async Task<IActionResult> DeleteWebApplicationConfiguration(
        Guid id,
        string configurationKey,
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(
            new DeleteWebApplicationConfigItemCommand { WebApplicationId = id, Key = configurationKey },
            cancellationToken);
        return NoContent();
    }

    [HttpPost("{serverId:guid}/containerAction")]
    public async Task<IActionResult> ExecuteContainerAction(Guid serverId, [FromQuery] string actionId)
    {
        await mediator.Send(new WebApplicationContainerActionCommand { Id = serverId, ActionId = actionId });
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteWebApplicationResource(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new DeleteWebApplicationResourceCommand { Id = id }, cancellationToken);
        return NoContent();
    }
}
