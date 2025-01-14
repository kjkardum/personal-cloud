using ApiExceptions.Exceptions;
using AutoBogus;
using Bogus;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.Services;
using Kjkardum.CloudyBack.Application.UseCases.User.Commands.Create;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Kjkardum.CloudyBack.Application.Tests.UseCases.User.Commands;

public class UserRegistrationCommandHandlerTests
{
    private readonly UserRegistrationCommandHandler _userRegistrationCommandHandler;
    private readonly IUserRepository _userRepository;
    private readonly ISignInService _signInService;

    public UserRegistrationCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _signInService = Substitute.For<ISignInService>();

        _userRegistrationCommandHandler = new UserRegistrationCommandHandler(_userRepository, _signInService);
    }

    [Fact]
    public async Task UserCreateCommandHandler_Valid_Request_Creates_New_User()
    {
        //Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<UserRegistrationCommand>();
        var createdUser = new Faker<Domain.Entities.User>()
            .RuleFor(t => t.Id, f => f.Random.Guid())
            .RuleFor(t => t.Email, _ => request.NormalizedEmail)
            .Generate();
        var requestor = new Faker<Domain.Entities.User>().RuleFor(t => t.Id, f => f.Random.Guid())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _userRepository.DoesUserExist(request.NormalizedEmail).Returns(false);
        _signInService.HashPassword(request.Password).Returns("hashedPassword");
        _userRepository
            .Create(Arg.Any<Domain.Entities.User>())
            .Returns(createdUser);

        // Act
        await _userRegistrationCommandHandler.Handle(request, cancellationToken);

        //Assert
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _userRepository.Received(1).DoesUserExist(request.NormalizedEmail);
        _signInService.Received(1).HashPassword(request.Password);
        await _userRepository.Received(1).Create(Arg.Is<Domain.Entities.User>(user =>
                user.Email == request.NormalizedEmail &&
                user.PasswordHash == "hashedPassword"
        ));
    }

    [Fact]
    public async Task UserCreateCommandHandler_Requestor_Is_Not_Existing_Throws_ForbiddenAccessException()
    {
        //Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<UserRegistrationCommand>();
        Domain.Entities.User? requestor = null;

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);

        // Act
        var act = async () => await _userRegistrationCommandHandler.Handle(request, cancellationToken);

        //Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _userRepository.DidNotReceive().DoesUserExist(request.NormalizedEmail);
        _signInService.DidNotReceive().HashPassword(request.Password);
        await _userRepository.DidNotReceive().Create(Arg.Any<Domain.Entities.User>());
    }

    [Fact]
    public async Task UserCreateCommandHandler_User_With_Same_Email_Already_Exists_Throws_EntityAlreadyExistsException()
    {
        //Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<UserRegistrationCommand>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(t => t.Id, f => f.Random.Guid())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _userRepository.DoesUserExist(request.NormalizedEmail).Returns(true);

        // Act
        var act = async () => await _userRegistrationCommandHandler.Handle(request, cancellationToken);

        //Assert
        await act.Should().ThrowAsync<EntityAlreadyExistsException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _userRepository.Received(1).DoesUserExist(request.NormalizedEmail);
        _signInService.DidNotReceive().HashPassword(request.Password);
        await _userRepository.DidNotReceive().Create(Arg.Any<Domain.Entities.User>());
    }

    [Fact]
    public async Task UserCreateCommandHandler_HashPassword_Returns_Null_Throws_BadRequestException()
    {
        //Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<UserRegistrationCommand>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(t => t.Id, f => f.Random.Guid())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _userRepository.DoesUserExist(request.NormalizedEmail).Returns(false);
        _signInService.HashPassword(request.Password).Returns((string?)null);

        // Act
        var act = async () => await _userRegistrationCommandHandler.Handle(request, cancellationToken);

        //Assert
        await act.Should().ThrowAsync<BadRequestException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _userRepository.Received(1).DoesUserExist(request.NormalizedEmail);
        _signInService.Received(1).HashPassword(request.Password);
        await _userRepository.DidNotReceive().Create(Arg.Any<Domain.Entities.User>());
    }
}
