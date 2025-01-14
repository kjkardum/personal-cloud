using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Kjkardum.CloudyBack.Application.UseCases.User.Commands.Login;
using Kjkardum.CloudyBack.Application.UseCases.User.Dto;

namespace Kjkardum.CloudyBack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Logs in existing user.
    /// </summary>
    /// <param name="request">Users login details.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>Logged in user token and data.</returns>
    /// <response code="200">Ok - logged in user.</response>
    /// <response code="401">Unauthorized - incorrect password for user.</response>
    [HttpPost("Login")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoggedInUserDto>> Login(
        UserLoginCommand request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            request,
            cancellationToken);

        return Ok(result);
    }
}
