using AutoMapper;
using Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Dto;
using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Mappings;

public class ResourceGroupProfile: Profile
{
    public ResourceGroupProfile()
    {
        CreateMap<ResourceGroup, ResourceGroupSimpleDto>();
        CreateMap<ResourceGroup, ResourceGroupDto>();
    }
}
