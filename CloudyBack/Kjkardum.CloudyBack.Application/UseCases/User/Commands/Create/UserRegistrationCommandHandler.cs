using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.Services;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.User.Commands.Create;

public class UserRegistrationCommandHandler(
    IUserRepository userRepository,
    ISignInService signInService)
    : IRequestHandler<UserRegistrationCommand>
{
    public async Task Handle(UserRegistrationCommand request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);

        if (requestor == null)
        {
            throw new ForbiddenAccessException("Samo trenutni korisnici mogu kreirati nove korisnike");
        }

        if (await userRepository.DoesUserExist(request.NormalizedEmail))
        {
            throw new EntityAlreadyExistsException("Korisnik s ovim emailom već postoji");
        }

        var passwordHash = signInService.HashPassword(request.Password);
        if (passwordHash == null)
        {
            throw new BadRequestException("Nije moguće procesirati lozinku");
        }

        var user = new Domain.Entities.User {
            Email = request.NormalizedEmail,
            PasswordHash = passwordHash,
            LastLogin = DateTime.UtcNow,
        };

        await userRepository.Create(user);
    }
}
