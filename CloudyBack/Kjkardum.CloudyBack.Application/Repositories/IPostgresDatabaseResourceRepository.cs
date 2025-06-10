using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Repositories;

public interface IPostgresDatabaseResourceRepository
{
    Task<PostgresDatabaseResource> Create(PostgresDatabaseResource postgresDatabaseResource);
    Task<PostgresDatabaseResource?> GetById(Guid id);
    Task<(IEnumerable<PostgresDatabaseResource> databases, int totalCount)> GetPaginated(PaginatedRequest pagination);
}

