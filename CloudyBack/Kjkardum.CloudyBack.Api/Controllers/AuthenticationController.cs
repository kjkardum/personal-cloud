using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Kjkardum.CloudyBack.Application.UseCases.User.Commands.Login;
using Kjkardum.CloudyBack.Application.UseCases.User.Dto;

namespace Kjkardum.CloudyBack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController(IMediator mediator, IConfiguration configuration) : ControllerBase
{
    private readonly int _jwtDurationInMinutes = configuration.GetValue<int>("Jwt__DurationInMinutes");
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

        SetCookies(result.Token);

        return Ok(result);
    }

    /// <summary>
    /// Logs out the current user.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>204 No Content.</returns>
    /// <response code="204">No Content - user logged out.</response>
    [HttpPost("Logout")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        SetCookies(string.Empty, true);
        return NoContent();
    }

    private void SetCookies(string jwt, bool expired = false)
    {
        var jwtCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
        };

        jwtCookieOptions.Expires = expired
            ? DateTimeOffset.UtcNow.AddDays(-1)
            : DateTimeOffset.UtcNow.AddMinutes(_jwtDurationInMinutes);
        Response.Cookies.Append("x-cloudy-token", jwt, jwtCookieOptions);
    }
}
