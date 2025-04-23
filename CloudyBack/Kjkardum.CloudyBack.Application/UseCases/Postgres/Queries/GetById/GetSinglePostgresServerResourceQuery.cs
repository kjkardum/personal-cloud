using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Queries.GetById;

public class GetSinglePostgresServerResourceQuery: IRequest<PostgresServerResourceDto>
{
    public Guid Id { get; set; }
}
