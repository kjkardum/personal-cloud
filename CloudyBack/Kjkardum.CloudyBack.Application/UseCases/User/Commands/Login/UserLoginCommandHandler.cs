using ApiExceptions.Exceptions;
using AutoMapper;
using MediatR;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.Services;
using Kjkardum.CloudyBack.Application.UseCases.User.Dto;

namespace Kjkardum.CloudyBack.Application.UseCases.User.Commands.Login;

public class UserLoginCommandHandler(
    IUserRepository userRepository,
    IMapper mapper,
    ISignInService signInService)
    : IRequestHandler<UserLoginCommand, LoggedInUserDto>
{
    public async Task<LoggedInUserDto> Handle(UserLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmail(request.NormalizedEmail);
        if (user == null)
        {
            throw new UnAuthorizedAccessException("Neispravni podaci za prijavu");
        }

        if (!signInService.CheckPasswordHash(user.PasswordHash, request.Password))
        {
            throw new UnAuthorizedAccessException("Neispravn podaci za prijavu");
        }

        var newUser = await userRepository.UpdateLastLogin(user);
        var result = mapper.Map<LoggedInUserDto>(newUser);

        result.Token = signInService.GenerateJwToken(newUser);

        return result;
    }
}
