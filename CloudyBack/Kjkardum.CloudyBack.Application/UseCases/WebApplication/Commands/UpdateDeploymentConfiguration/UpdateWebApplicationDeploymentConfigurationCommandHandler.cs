using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using Kjkardum.CloudyBack.Domain.Enums;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.WebApplication.Commands.UpdateDeploymentConfiguration;

public class UpdateWebApplicationDeploymentConfigurationCommandHandler(
    IWebApplicationResourceRepository webApplicationResourceRepository,
    IBaseResourceRepository baseResourceRepository,
    IWebApplicationClient webApplicationClient) : IRequestHandler<UpdateWebApplicationDeploymentConfigurationCommand>
{
    public async Task Handle(
        UpdateWebApplicationDeploymentConfigurationCommand request,
        CancellationToken cancellationToken)
    {
        var webApplicationResource = await webApplicationResourceRepository.GetById(request.Id);
        if (webApplicationResource == null)
        {
            throw new KeyNotFoundException($"Web application resource with ID {request.Id} not found.");
        }

        webApplicationResource.BuildCommand = request.BuildCommand;
        webApplicationResource.StartupCommand = request.StartupCommand;
        webApplicationResource.Port = request.Port;

        await webApplicationResourceRepository.Update(webApplicationResource);

        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(UpdateWebApplicationDeploymentConfigurationCommand),
                ActionDisplayText
                    = $"Update deployment configuration for Web application {webApplicationResource.Name}",
                ResourceId = webApplicationResource.Id
            });

        var variablesAsList = webApplicationResource.Configuration!.Select(t => $"{t.Key}={t.Value}").ToList();
        var networksAsList = webApplicationResource.VirtuaLNetworks!.Select(t => t.VirtualNetworkId).ToList();

        switch (webApplicationResource.SourceType)
        {
            case WebApplicationSourceType.PublicGitClone:
                await baseResourceRepository.LogResourceAction(new AuditLogEntry
                    {
                        ActionName = nameof(UpdateWebApplicationDeploymentConfigurationCommand),
                        ActionDisplayText = $"Deploying git repo configuration to {webApplicationResource.Name}",
                        ResourceId = webApplicationResource.Id
                    });
                try
                {
                    await webApplicationClient.BuildAndRunWebApplicationUsingGitRepo(
                        webApplicationResource.Id,
                        webApplicationResource.SourcePath,
                        webApplicationResource.BuildCommand,
                        webApplicationResource.StartupCommand,
                        webApplicationResource.Port,
                        variablesAsList,
                        networksAsList);
                    await baseResourceRepository.LogResourceAction(new AuditLogEntry
                        {
                            ActionName = nameof(UpdateWebApplicationDeploymentConfigurationCommand),
                            ActionDisplayText
                                = $"Successfully deployed git repo configuration to {webApplicationResource.Name}",
                            ResourceId = webApplicationResource.Id
                        });
                } catch (Exception ex)
                {
                    await baseResourceRepository.LogResourceAction(new AuditLogEntry
                        {
                            ActionName = nameof(UpdateWebApplicationDeploymentConfigurationCommand),
                            ActionDisplayText =
                                $"Failed to deploy git repo configuration to {webApplicationResource.Name}: {ex.Message}",
                            ResourceId = webApplicationResource.Id
                        });
                    throw;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
