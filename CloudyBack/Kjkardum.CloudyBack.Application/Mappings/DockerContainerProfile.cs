using AutoMapper;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Mappings;

public class DockerContainerProfile: Profile
{
    public DockerContainerProfile()
    {
        CreateMap<DockerContainer, ContainerDto>();
    }
}
