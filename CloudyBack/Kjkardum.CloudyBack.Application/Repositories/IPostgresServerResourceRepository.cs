using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Repositories;

public interface IPostgresServerResourceRepository
{
    Task<PostgresServerResource> Create(PostgresServerResource postgresServerResource);
    Task<PostgresServerResource?> GetById(Guid id);
    Task<(IEnumerable<PostgresServerResource>, int)> GetPaginated(PaginatedRequest pagination);
    Task Delete(PostgresServerResource postgresServerResource);
}
