using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetGrafanaConfiguration;

public class GetGrafanaConfigurationQueryHandler(
    IKeyValueRepository keyValueRepository) : IRequestHandler<GetGrafanaConfigurationQuery, GrafanaConfigurationDto>
{
    public async Task<GrafanaConfigurationDto> Handle(
        GetGrafanaConfigurationQuery request,
        CancellationToken cancellationToken)
    {
        var grafanaConnectionKeyValue = await keyValueRepository.GetValueAsync(
            KeyValueTableEntry.GrafanaPublicConnection);
        if (grafanaConnectionKeyValue == null)
        {
            return new GrafanaConfigurationDto { Created = false };
        }

        var uriObject = new Uri(grafanaConnectionKeyValue);
        return new GrafanaConfigurationDto
        {
            Created = true,
            Host = uriObject.Host,
            UseHttps = uriObject.Scheme == "https"
        };
    }
}
