using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Repositories;

public interface IBaseResourceRepository
{
    Task<(IEnumerable<BaseResource>, int)> GetPaginated(PaginatedRequest pagination);
}
