using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Queries.GetDatabaseExtensions;

public class GetPostgresDatabaseExtensionsQuery: IRequest<PostgresQueryResultDto>
{
    public Guid DatabaseId { get; set; }
}
