using Kjkardum.CloudyBack.Application.Request;
using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Dto;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.ResourceGroup.Queries;

public class GetAllResourceGroupsQuery(PaginatedRequest request): IRequest<PaginatedResponse<ResourceGroupDto>>
{
    public PaginatedRequest Pagination { get; } = request;
}
