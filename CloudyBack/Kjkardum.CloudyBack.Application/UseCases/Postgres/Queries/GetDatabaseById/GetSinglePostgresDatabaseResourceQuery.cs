using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Queries.GetDatabaseById;

public class GetSinglePostgresDatabaseResourceQuery: IRequest<PostgresDatabaseResourceDto>
{
    public Guid Id { get; set; }
}
