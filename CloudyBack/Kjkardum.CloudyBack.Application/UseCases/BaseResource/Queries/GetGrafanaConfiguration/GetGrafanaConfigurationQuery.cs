using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetGrafanaConfiguration;

public class GetGrafanaConfigurationQuery : IRequest<GrafanaConfigurationDto>;
