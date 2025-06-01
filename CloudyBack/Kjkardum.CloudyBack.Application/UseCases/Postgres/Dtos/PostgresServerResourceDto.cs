using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;

public class PostgresServerResourceDto : VirtualNetworkableBaseResourceDto
{
    public List<PostgresDatabaseSimpleResourceDto> PostgresDatabaseResources { get; set; }
}
