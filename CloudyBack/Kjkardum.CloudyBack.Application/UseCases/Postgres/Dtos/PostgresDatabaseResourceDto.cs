using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;

public class PostgresDatabaseResourceDto : BaseResourceDto
{
    public string DatabaseName { get; set; }
    public string AdminUsername { get; set; }
}
