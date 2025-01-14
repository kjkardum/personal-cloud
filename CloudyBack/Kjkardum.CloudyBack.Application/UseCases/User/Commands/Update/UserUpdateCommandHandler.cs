using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.Services;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.User.Commands.Update;

public class UserUpdateCommandHandler(
    IUserRepository userRepository,
    ISignInService signInService) : IRequestHandler<UserUpdateCommand>
{
    public async Task Handle(UserUpdateCommand request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        if (requestor == null)
        {
            throw new ForbiddenAccessException("Samo trenutni korisnici mogu ažurirati podatke korisnika");
        }
        var user = await userRepository.GetByUserId(request.Id);
        if (user == null)
        {
            throw new EntityNotFoundException("Korisnik nije pronađen");
        }
        if (request.NewPassword != null)
        {
            var passwordHash = signInService.HashPassword(request.NewPassword);
            if (passwordHash == null)
            {
                throw new BadRequestException("Nije moguće procesirati lozinku");
            }
            user.PasswordHash = passwordHash;
        }

        await userRepository.Update(user);
    }
}
