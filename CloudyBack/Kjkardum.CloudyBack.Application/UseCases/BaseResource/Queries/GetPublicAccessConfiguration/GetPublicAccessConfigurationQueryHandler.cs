using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.GetPublicAccessConfiguration;

public class GetPublicAccessConfigurationQueryHandler(
    IKeyValueRepository keyValueRepository)
    : IRequestHandler<GetPublicAccessConfigurationQuery, PublicAccessConfigurationDto>
{
    public async Task<PublicAccessConfigurationDto> Handle(
        GetPublicAccessConfigurationQuery request,
        CancellationToken cancellationToken)
    {
        var publicAccessConnectionKeyValue = await keyValueRepository.GetValueAsync(
            KeyValueTableEntry.PublicHttpsAccessConnection);
        if (publicAccessConnectionKeyValue == null)
        {
            return new PublicAccessConfigurationDto { Created = false };
        }

        var uriObject = new Uri(publicAccessConnectionKeyValue);
        return new PublicAccessConfigurationDto
        {
            Created = true,
            Host = uriObject.Host,
        };
    }
}
