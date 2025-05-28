using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Dto;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Queries.GetAll;

public class GetAllResourceGroupsQuery(PaginatedRequest request): IRequest<PaginatedResponse<ResourceGroupSimpleDto>>
{
    public PaginatedRequest Pagination { get; } = request;
}
