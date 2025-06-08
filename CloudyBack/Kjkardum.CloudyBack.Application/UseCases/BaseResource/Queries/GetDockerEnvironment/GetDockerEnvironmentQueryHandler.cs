using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetDockerEnvironment;

public class GetDockerEnvironmentQueryHandler(
    IGeneralContainerStatusClient client) : IRequestHandler<GetDockerEnvironmentQuery, DockerEnvironment>
{
    public async Task<DockerEnvironment> Handle(GetDockerEnvironmentQuery request, CancellationToken cancellationToken)
    {
        var environment = await client.GetDockerEnvironmentAsync();
        return environment;
    }
}
