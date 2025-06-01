using AutoMapper;
using Kjkardum.CloudyBack.Application.UseCases.WebApplication.Dto;
using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Mappings;

public class WebApplicationProfile : Profile
{
    public WebApplicationProfile()
    {
        CreateMap<WebApplicationResource, WebApplicationResourceDto>()
            .ForMember(
                dest => dest.VirtualNetworks,
                opt => opt.MapFrom(src => src.VirtuaLNetworks!
                    .Select(vn => vn.VirtualNetwork)));
        CreateMap<WebApplicationConfigurationEntry, WebApplicationConfigurationEntryDto>();
    }
}
