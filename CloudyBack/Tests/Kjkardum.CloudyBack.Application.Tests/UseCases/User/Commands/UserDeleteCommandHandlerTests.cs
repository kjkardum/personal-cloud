using ApiExceptions.Exceptions;
using AutoBogus;
using Bogus;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.User.Commands.Delete;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Kjkardum.CloudyBack.Application.Tests.UseCases.User.Commands
{
    public class UserDeleteCommandHandlerTests
    {
        private readonly UserDeleteCommandHandler _userDeleteCommandHandler;
        private readonly IUserRepository _userRepository;

        public UserDeleteCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();

            _userDeleteCommandHandler = new UserDeleteCommandHandler(_userRepository);
        }

        [Fact]
        public async Task Handle_Valid_Request_Deletes_User()
        {
            // Arrange
            var cancellationToken = default(CancellationToken);
            var request = AutoFaker.Generate<UserDeleteCommand>();
            var requestor = new Faker<Domain.Entities.User>()
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .Generate();
            var user = new Faker<Domain.Entities.User>()
                .RuleFor(u => u.Id, request.Id)
                .Generate();

            _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
            _userRepository.GetByUserId(request.Id).Returns(user);

            // Act
            await _userDeleteCommandHandler.Handle(request, cancellationToken);

            // Assert
            await _userRepository.Received(1).GetByUserId(request.RequestorId);
            await _userRepository.Received(1).GetByUserId(request.Id);
            await _userRepository.Received(1).Delete(user);
        }

        [Fact]
        public async Task Handle_Requestor_Is_Not_Existing_Throws_ForbiddenAccessException()
        {
            // Arrange
            var cancellationToken = default(CancellationToken);
            var request = AutoFaker.Generate<UserDeleteCommand>();
            Domain.Entities.User? requestor = null;

            _userRepository.GetByUserId(request.RequestorId).Returns(requestor);

            // Act
            var act = async () => await _userDeleteCommandHandler.Handle(request, cancellationToken);

            // Assert
            await act.Should().ThrowAsync<ForbiddenAccessException>();
            await _userRepository.Received(1).GetByUserId(request.RequestorId);
            await _userRepository.DidNotReceive().GetByUserId(request.Id);
            await _userRepository.DidNotReceive().Delete(Arg.Any<Domain.Entities.User>());
        }

        [Fact]
        public async Task Handle_User_Not_Found_Throws_EntityNotFoundException()
        {
            // Arrange
            var cancellationToken = default(CancellationToken);
            var request = AutoFaker.Generate<UserDeleteCommand>();
            var requestor = new Faker<Domain.Entities.User>()
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .Generate();

            _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
            _userRepository.GetByUserId(request.Id).Returns((Domain.Entities.User?)null);

            // Act
            var act = async () => await _userDeleteCommandHandler.Handle(request, cancellationToken);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
            await _userRepository.Received(1).GetByUserId(request.RequestorId);
            await _userRepository.Received(1).GetByUserId(request.Id);
            await _userRepository.DidNotReceive().Delete(Arg.Any<Domain.Entities.User>());
        }

    }
}
