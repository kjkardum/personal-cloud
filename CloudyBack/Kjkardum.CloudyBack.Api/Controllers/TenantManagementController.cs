using Kjkardum.CloudyBack.Api.Services;
using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.User.Commands.Create;
using Kjkardum.CloudyBack.Application.UseCases.User.Commands.Delete;
using Kjkardum.CloudyBack.Application.UseCases.User.Commands.Update;
using Kjkardum.CloudyBack.Application.UseCases.User.Dto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kjkardum.CloudyBack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TenantManagementController(
    IMediator mediator,
    IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost("createUser")]
    public async Task<ActionResult> CreateUser(
        UserRegistrationCommand request,
        CancellationToken cancellationToken = default)
    {
        request.RequestorId = (Guid)authenticationService.GetUserId()!;

        await mediator.Send(request, cancellationToken);
        return Ok();
    }

    [HttpPut("updateUser/{id:guid}")]
    public async Task<ActionResult> UpdateUser(
        Guid id,
        UserUpdateCommand request,
        CancellationToken cancellationToken = default)
    {
        request.RequestorId = (Guid)authenticationService.GetUserId()!;
        request.Id = id;

        await mediator.Send(request, cancellationToken);
        return Ok();
    }

    [HttpDelete("deleteUser/{id:guid}")]
    public async Task<ActionResult> DeleteUser(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var request = new UserDeleteCommand
        {
            RequestorId = (Guid)authenticationService.GetUserId()!,
            Id = id
        };

        await mediator.Send(request, cancellationToken);
        return Ok();
    }
}
