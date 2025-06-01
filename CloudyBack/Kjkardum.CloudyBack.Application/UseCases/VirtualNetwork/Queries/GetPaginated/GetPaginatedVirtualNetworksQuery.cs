using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.VirtualNetwork.Queries.GetPaginated;

public class GetPaginatedVirtualNetworksQuery(PaginatedRequest request): IRequest<PaginatedResponse<VirtualNetworkSimpleDto>>
{
    public PaginatedRequest Pagination { get; } = request;
}
