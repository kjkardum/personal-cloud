using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.WebApplication.Commands.Delete;

public class DeleteWebApplicationResourceCommandHandler(
    IGeneralContainerStatusClient client,
    IReverseProxyClient reverseProxyClient,
    IBaseResourceRepository baseResourceRepository,
    IWebApplicationResourceRepository repository) : IRequestHandler<DeleteWebApplicationResourceCommand>
{
    public async Task Handle(DeleteWebApplicationResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await repository.GetById(request.Id);
        if (resource == null)
        {
            throw new EntityNotFoundException($"Web application resource with ID {request.Id} not found.");
        }
        foreach (var resourcePublicProxyConfiguration in resource.PublicProxyConfigurations!)
        {
            await reverseProxyClient.RemoveProxyConfiguration(
                resourcePublicProxyConfiguration.Id,
                resourcePublicProxyConfiguration.Port,
                resourcePublicProxyConfiguration.Domain,
                resourcePublicProxyConfiguration.UseHttps);
        }

        await repository.Delete(resource);

        await baseResourceRepository.LogResourceAction(new AuditLogEntry
            {
                ActionName = nameof(DeleteWebApplicationResourceCommand),
                ActionDisplayText = $"Delete resource {resource.Name}",
                ResourceId = resource.ResourceGroupId
            });
        await client.DeleteContainerAsync(request.Id);
    }
}
