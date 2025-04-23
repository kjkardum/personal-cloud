using AutoMapper;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Mappings;

public class BaseResourceProfile: Profile
{
    public BaseResourceProfile()
    {
        CreateMap<BaseResource, BaseResourceDto>()
            .ForMember(dest => dest.ResourceType, opt => opt.MapFrom(src => src.GetType().Name));
    }
}
