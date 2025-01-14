using AutoMapper;
using Kjkardum.CloudyBack.Application.UseCases.User.Dto;
using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Mappings;

public class UserProfile: Profile
{
    public UserProfile()
    {
        CreateMap<User, LoggedInUserDto>()
            .ForMember(t => t.Token, opt => opt.Ignore());

        CreateMap<User, UserDto>();
    }
}
