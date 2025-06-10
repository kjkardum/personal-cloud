using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using Kjkardum.CloudyBack.Application.UseCases.VirtualNetwork.Commands.Connect;
using Kjkardum.CloudyBack.Application.UseCases.VirtualNetwork.Commands.Create;
using Kjkardum.CloudyBack.Application.UseCases.VirtualNetwork.Dto;
using Kjkardum.CloudyBack.Application.UseCases.VirtualNetwork.Queries.GetById;
using Kjkardum.CloudyBack.Application.UseCases.VirtualNetwork.Queries.GetPaginated;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kjkardum.CloudyBack.Api.Controllers.Resources;

[Authorize]
[ApiController]
[Route("api/resource/[controller]")]
public class VirtualNetworkResourceController(IMediator mediator): ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<VirtualNetworkSimpleDto>>> GetAll(
        [FromQuery] PaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetPaginatedVirtualNetworksQuery(request), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VirtualNetworkResourceDto>> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetVirtualNetworkByIdQuery{ Id = id }, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<VirtualNetworkSimpleDto>> CreateVirtualNetworkResource(
        CreateVirtualNetworkResourceCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }

    [HttpPost("{id:guid}/connect")]
    public async Task<IActionResult> ConnectVirtualNetwork(
        Guid id,
        ConnectVirtualNetworkCommand command,
        CancellationToken cancellationToken = default)
    {
        command.NetworkId = id;
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
