using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetPublicAccessConfiguration;

public class GetPublicAccessConfigurationQuery : IRequest<PublicAccessConfigurationDto>;
