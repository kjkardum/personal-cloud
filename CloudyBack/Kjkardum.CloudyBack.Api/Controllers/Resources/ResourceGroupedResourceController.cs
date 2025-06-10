using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetContainer;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetPaginated;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetResourceGrouped;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kjkardum.CloudyBack.Api.Controllers.Resources;

[Authorize]
[ApiController]
[Route("api/resource/[controller]")]
public class ResourceGroupedResourceController(IMediator mediator): ControllerBase
{
    [HttpGet("{resourceId:guid}")]
    public async Task<ActionResult<ResourceGroupedBaseResourceDto>> GetResource(Guid resourceId)
    {
        var result = await mediator.Send(new GetResourceGroupedBaseResourceQuery { Id = resourceId });
        return Ok(result);
    }
}
