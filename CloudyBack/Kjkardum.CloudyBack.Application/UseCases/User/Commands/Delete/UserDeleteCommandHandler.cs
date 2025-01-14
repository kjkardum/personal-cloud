using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Repositories;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.User.Commands.Delete;

public class UserDeleteCommandHandler(IUserRepository userRepository) : IRequestHandler<UserDeleteCommand>
{
    public async Task Handle(UserDeleteCommand request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        if (requestor == null)
        {
            throw new ForbiddenAccessException("Samo trenutni korisnici mogu brisati korisnike");
        }
        var user = await userRepository.GetByUserId(request.Id);
        if (user == null)
        {
            throw new EntityNotFoundException("Korisnik nije pronađen");
        }

        await userRepository.Delete(user);
    }
}
