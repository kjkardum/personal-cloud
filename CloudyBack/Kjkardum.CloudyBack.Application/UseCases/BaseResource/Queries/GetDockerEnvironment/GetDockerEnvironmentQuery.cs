using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetDockerEnvironment;

public class GetDockerEnvironmentQuery : IRequest<DockerEnvironment>;
