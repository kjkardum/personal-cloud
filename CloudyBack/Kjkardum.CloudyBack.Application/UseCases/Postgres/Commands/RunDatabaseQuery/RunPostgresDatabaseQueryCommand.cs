using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using MediatR;
using System.Text.Json.Serialization;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Commands.RunDatabaseQuery;

public class RunPostgresDatabaseQueryCommand: IRequest<PostgresQueryResultDto>
{
    public string Query { get; set; }
    [JsonIgnore] public Guid DatabaseId { get; set; }
}
