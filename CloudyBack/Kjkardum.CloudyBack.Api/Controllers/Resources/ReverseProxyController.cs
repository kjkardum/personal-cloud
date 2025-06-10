using Kjkardum.CloudyBack.Application.UseCases.ReverseProxy.Commands.Connect;
using Kjkardum.CloudyBack.Application.UseCases.ReverseProxy.Commands.Disconnect;
using Kjkardum.CloudyBack.Application.UseCases.ReverseProxy.Dto;
using Kjkardum.CloudyBack.Application.UseCases.ReverseProxy.Queries.PreCheckDns;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kjkardum.CloudyBack.Api.Controllers.Resources;

[Authorize]
[ApiController]
[Route("api/resource/[controller]")]
public class ReverseProxyController(IMediator mediator) : ControllerBase
{
    [HttpPost("connect/{resourceId:guid}")]
    public async Task<IActionResult> ConnectToResource(
        Guid resourceId,
        ConnectReverseProxyCommand command,
        CancellationToken cancellationToken = default)
    {
        command.ResourceId = resourceId;
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("disconnect/{resourceId:guid}")]
    public async Task<IActionResult> DisconnectFromResource(
        Guid resourceId,
        DisconnectReverseProxyCommand command,
        CancellationToken cancellationToken = default)
    {
        command.ResourceId = resourceId;
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpGet("preCheckDns")]
    public async Task<ActionResult<DnsCheckDto>> PreCheckDns(
        [FromQuery] string url,
        [FromQuery] string myAdminUrl,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new PreCheckDnsQuery{ Url = url, AdminUrl = myAdminUrl }, cancellationToken);
        return Ok(result);
    }
}
