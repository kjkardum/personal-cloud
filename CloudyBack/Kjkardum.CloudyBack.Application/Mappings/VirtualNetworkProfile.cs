using AutoMapper;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using Kjkardum.CloudyBack.Application.UseCases.VirtualNetwork.Dto;
using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Mappings;

public class VirtualNetworkProfile: Profile
{
    public VirtualNetworkProfile()
    {
        CreateMap<VirtualNetworkableBaseResource, VirtualNetworkableBaseResourceDto>()
            .ForMember(dest => dest.ResourceType, opt => opt.MapFrom(src => src.GetType().Name))
            .ForMember(dest => dest.ResourceGroupName, opt => opt.MapFrom(src => src.ResourceGroup.Name))
            .ForMember(
                dest => dest.VirtualNetworks,
                opt => opt.MapFrom(src => src.VirtuaLNetworks!
                    .Select(vn => vn.VirtualNetwork)));
        CreateMap<VirtualNetworkResource, VirtualNetworkSimpleDto>();

        CreateMap<VirtualNetworkResource, VirtualNetworkResourceDto>()
            .ForMember(
                dest => dest.Resources,
                opt => opt.MapFrom(t =>
                    (t.NetworkConnections ?? new List<VirtualNetworkConnection>()).Select(t => t.Resource)));
    }
}
