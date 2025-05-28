using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Commands.Create;
using Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Dto;
using Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Queries;
using Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Queries.GetAll;
using Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Kjkardum.CloudyBack.Api.Controllers.Resources;

[ApiController]
[Route("api/resource/[controller]")]
public class ResourceGroupController(IMediator mediator): ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ResourceGroupSimpleDto>> Create(
        CreateResourceGroupCommand request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<ResourceGroupSimpleDto>>> GetAll(
        [FromQuery] PaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllResourceGroupsQuery(request), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ResourceGroupDto>> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetResourceGroupByIdQuery { Id = id }, cancellationToken);
        return Ok(result);
    }
}
