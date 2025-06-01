using AutoMapper;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.WebApplication.Dto;
using Kjkardum.CloudyBack.Domain.Entities;
using Kjkardum.CloudyBack.Domain.Enums;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.WebApplication.Commands.Create;

public class CreateWebApplicationResourceCommandHandler(
    IWebApplicationResourceRepository webApplicationResourceRepository,
    IBaseResourceRepository baseResourceRepository,
    IMapper mapper) :
        IRequestHandler<CreateWebApplicationResourceCommand, WebApplicationResourceDto>
{
    public async Task<WebApplicationResourceDto> Handle(
        CreateWebApplicationResourceCommand request,
        CancellationToken cancellationToken)
    {
        var webApplicationResource = new WebApplicationResource
        {
            Name = request.WebApplicationName,
            ResourceGroupId = request.ResourceGroupId,
            SourcePath = request.SourcePath,
            SourceType = request.SourceType,
            Port = 8080,
        };
        webApplicationResource = await webApplicationResourceRepository.Create(webApplicationResource);

        /*await observabilityClient.AttachCollector(webApplicationResource.Id, request.WebApplicationName);*/

        /*await webApplicationClient.CreateClusterAsync(
            webApplicationResource.Id,
            webApplicationResource.Name);*/

        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(CreateWebApplicationResourceCommand),
                ActionDisplayText = $"Create new Web application {request.WebApplicationName}",
                ResourceId = webApplicationResource.Id
            });

        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(CreateWebApplicationResourceCommand),
                ActionDisplayText = $"Create new resource {request.WebApplicationName}",
                ResourceId = request.ResourceGroupId
            });

        return mapper.Map<WebApplicationResourceDto>(webApplicationResource);
    }
}
