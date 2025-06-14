using AutoMapper;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Mappings;

public class PostgresProfile: Profile
{
    public PostgresProfile()
    {
        CreateMap<PostgresServerResource, PostgresServerResourceDto>()
            .ForMember(
                dest => dest.VirtualNetworks,
                opt => opt.MapFrom(src => src.VirtuaLNetworks!
                    .Select(vn => vn.VirtualNetwork)));
        CreateMap<PostgresDatabaseResource, PostgresDatabaseSimpleResourceDto>();
        CreateMap<PostgresDatabaseResource, PostgresDatabaseResourceDto>()
            .ForMember(dest => dest.ServerId, opt => opt.MapFrom(src => src.PostgresDatabaseServerResource!.Id))
            .ForMember(dest => dest.ServerName, opt => opt.MapFrom(src => src.PostgresDatabaseServerResource!.Name));
    }
}
