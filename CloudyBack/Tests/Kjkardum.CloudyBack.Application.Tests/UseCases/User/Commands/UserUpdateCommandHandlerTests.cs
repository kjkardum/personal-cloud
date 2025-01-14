using ApiExceptions.Exceptions;
using AutoBogus;
using Bogus;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.Services;
using Kjkardum.CloudyBack.Application.UseCases.User.Commands.Update;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Kjkardum.CloudyBack.Application.Tests.UseCases.User.Commands;

public class UserUpdateCommandHandlerTests
{
    private readonly UserUpdateCommandHandler _userUpdateCommandHandler;
    private readonly IUserRepository _userRepository;
    private readonly ISignInService _signInService;

    public UserUpdateCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _signInService = Substitute.For<ISignInService>();

        _userUpdateCommandHandler = new UserUpdateCommandHandler(_userRepository, _signInService);
    }

    [Theory]
    [InlineData(null, 0)]
    [InlineData("password", 1)]
    public async Task Handle_Valid_Request_Updates_User(string? password, int expectedCallsToSignInService)
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<UserUpdateCommand>();
        request.NewPassword = password;
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .Generate();
        var user = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, request.Id)
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _userRepository.GetByUserId(request.Id).Returns(user);
        _signInService.HashPassword(request.NewPassword).Returns("hashedPassword");

        // Act
        await _userUpdateCommandHandler.Handle(request, cancellationToken);

        // Assert
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _userRepository.Received(1).GetByUserId(request.Id);
        _signInService.Received(expectedCallsToSignInService).HashPassword(request.NewPassword);
        await _userRepository.Received(1).Update(Arg.Is<Domain.Entities.User>(u =>
            u.PasswordHash == (expectedCallsToSignInService > 0 ? "hashedPassword" : user.PasswordHash)
        ));
    }

    [Fact]
    public async Task Handle_Requestor_Is_Not_Existing_Throws_ForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<UserUpdateCommand>();

        _userRepository.GetByUserId(request.RequestorId).Returns((Domain.Entities.User?)null);

        // Act
        var act = async () => await _userUpdateCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _userRepository.DidNotReceive().GetByUserId(request.Id);
        _signInService.DidNotReceive().HashPassword(request.NewPassword);
        await _userRepository.DidNotReceive().Update(Arg.Any<Domain.Entities.User>());
    }

    [Fact]
    public async Task Handle_User_Not_Found_Throws_EntityNotFoundException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<UserUpdateCommand>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _userRepository.GetByUserId(request.Id).Returns((Domain.Entities.User?)null);

        // Act
        var act = async () => await _userUpdateCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _userRepository.Received(1).GetByUserId(request.Id);
        _signInService.DidNotReceive().HashPassword(request.NewPassword);
        await _userRepository.DidNotReceive().Update(Arg.Any<Domain.Entities.User>());
    }
    [Fact]
    public async Task Handle_HashPassword_Returns_Null_Throws_BadRequestException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<UserUpdateCommand>();
        request.NewPassword = "password";
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .Generate();
        var user = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, request.Id)
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _userRepository.GetByUserId(request.Id).Returns(user);
        _signInService.HashPassword(request.NewPassword).Returns((string?)null);

        // Act
        var act = async () => await _userUpdateCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _userRepository.Received(1).GetByUserId(request.Id);
        _signInService.Received(1).HashPassword(request.NewPassword);
        await _userRepository.DidNotReceive().Update(Arg.Any<Domain.Entities.User>());
    }
}
