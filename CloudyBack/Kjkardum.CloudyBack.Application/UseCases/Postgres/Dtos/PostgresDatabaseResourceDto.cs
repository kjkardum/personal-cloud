using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;

public class PostgresDatabaseResourceDto: ResourceGroupedBaseResourceDto
{
    public string DatabaseName { get; set; }
    public string AdminUsername { get; set; }
    public string ServerName { get; set; }
    public string ServerId { get; set; }
}
