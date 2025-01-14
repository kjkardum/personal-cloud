using ApiExceptions.Exceptions;
using AutoBogus;
using AutoMapper;
using Bogus;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.Services;
using Kjkardum.CloudyBack.Application.UseCases.User.Commands.Login;
using Kjkardum.CloudyBack.Application.UseCases.User.Dto;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Kjkardum.CloudyBack.Application.Tests.UseCases.User.Commands;

public class UserLoginCommandHandlerTests
{
    private readonly UserLoginCommandHandler _userLoginCommandHandler;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ISignInService _signInService;

    public UserLoginCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _signInService = Substitute.For<ISignInService>();

        _userLoginCommandHandler = new UserLoginCommandHandler(_userRepository, _mapper, _signInService);
    }

    [Fact]
    public async Task Handle_Valid_Request_Returns_LoggedInUserDto()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<UserLoginCommand>();
        var user = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.Email, _ => request.NormalizedEmail)
            .RuleFor(u => u.PasswordHash, _ => "hashedPassword")
            .Generate();

        var loggedInUserDto = new Faker<LoggedInUserDto>()
            .RuleFor(l => l.Email, user.Email)
            .Generate();

        _userRepository.GetByEmail(request.NormalizedEmail).Returns(user);
        _signInService.CheckPasswordHash(user.PasswordHash, request.Password).Returns(true);
        _userRepository.UpdateLastLogin(user).Returns(user);
        _mapper.Map<LoggedInUserDto>(user).Returns(loggedInUserDto);
        _signInService.GenerateJwToken(user).Returns("jwtToken");

        // Act
        var result = await _userLoginCommandHandler.Handle(request, cancellationToken);

        // Assert
        await _userRepository.Received(1).GetByEmail(request.NormalizedEmail);
        _signInService.Received(1).CheckPasswordHash(user.PasswordHash, request.Password);
        await _userRepository.Received(1).UpdateLastLogin(user);
        _mapper.Received(1).Map<LoggedInUserDto>(user);
        _signInService.Received(1).GenerateJwToken(user);

        result.Should().NotBeNull();
        result.Email.Should().Be(loggedInUserDto.Email);
        result.Token.Should().Be("jwtToken");
    }

    [Fact]
    public async Task Handle_User_Does_Not_Exist_Throws_UnAuthorizedAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<UserLoginCommand>();

        _userRepository.GetByEmail(request.NormalizedEmail).Returns((Domain.Entities.User?)null);

        // Act
        var act = async () => await _userLoginCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<UnAuthorizedAccessException>();
        await _userRepository.Received(1).GetByEmail(request.NormalizedEmail);
        _signInService.DidNotReceive().CheckPasswordHash(Arg.Any<string>(), Arg.Any<string>());
        await _userRepository.DidNotReceive().UpdateLastLogin(Arg.Any<Domain.Entities.User>());
        _mapper.DidNotReceive().Map<LoggedInUserDto>(Arg.Any<Domain.Entities.User>());
        _signInService.DidNotReceive().GenerateJwToken(Arg.Any<Domain.Entities.User>());
    }

    [Fact]
    public async Task Handle_Invalid_Password_Throws_UnAuthorizedAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<UserLoginCommand>();
        var user = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.Email, _ => request.NormalizedEmail)
            .RuleFor(u => u.PasswordHash, _ => "hashedPassword")
            .Generate();

        _userRepository.GetByEmail(request.NormalizedEmail).Returns(user);
        _signInService.CheckPasswordHash(user.PasswordHash, request.Password).Returns(false);

        // Act
        var act = async () => await _userLoginCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<UnAuthorizedAccessException>();
        await _userRepository.Received(1).GetByEmail(request.NormalizedEmail);
        _signInService.Received(1).CheckPasswordHash(user.PasswordHash, request.Password);
        await _userRepository.DidNotReceive().UpdateLastLogin(Arg.Any<Domain.Entities.User>());
        _mapper.DidNotReceive().Map<LoggedInUserDto>(Arg.Any<Domain.Entities.User>());
        _signInService.DidNotReceive().GenerateJwToken(Arg.Any<Domain.Entities.User>());
    }
}
