using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;

public class PostgresServerResourceDto : ResourceGroupedBaseResourceDto
{
    public List<PostgresDatabaseResourceDto> PostgresDatabaseResources { get; set; }
}
