using AutoBogus;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Kjkardum.CloudyBack.Api.Controllers;
using Kjkardum.CloudyBack.Api.Services;
using Kjkardum.CloudyBack.Application.UseCases.User.Commands.Create;
using Kjkardum.CloudyBack.Application.UseCases.User.Commands.Login;
using Kjkardum.CloudyBack.Application.UseCases.User.Dto;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Kjkardum.CloudyBack.Api.Tests.Controllers;

public class AuthenticationControllerTests
{
    private readonly AuthenticationController _controller;
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public AuthenticationControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Jwt:DurationInMinutes", "60" }
                })
            .Build();
        _controller = new AuthenticationController(_mediator, _configuration);
    }

    [Fact]
    public async Task Login_User_Returns_LoggedInUserDto()
    {
        //Arrange
        var dto = AutoFaker.Generate<UserLoginCommand>();

        var cancellationToken = default(CancellationToken);
        var response = AutoFaker.Generate<LoggedInUserDto>();

        _mediator.Send(
            Arg.Is<UserLoginCommand>(
                x => x.Email == dto.Email),
            cancellationToken)
            .Returns(response);

        //Act
        var result = await _controller.Login(
            dto,
            cancellationToken);

        //Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var responseResult = result.Result as OkObjectResult;
        // Controller method ActionResult generic return type doesn't ensure the type of actual value returned if used with OkObjectResult
        var controllerMethodReturnType = _controller
            .GetType()
            .GetMethod(nameof(_controller.Login))!
            .ReturnType // Task<ActionResult<Type>>
            .GetGenericArguments()[0] // ActionResult<Type>
            .GetGenericArguments()[0]; // Type
        responseResult.Should().NotBeNull();
        responseResult!.Value.Should().NotBeNull();
        responseResult!.Value.Should().BeEquivalentTo(response);
        responseResult!.Value.Should().BeOfType(controllerMethodReturnType);
    }
}
