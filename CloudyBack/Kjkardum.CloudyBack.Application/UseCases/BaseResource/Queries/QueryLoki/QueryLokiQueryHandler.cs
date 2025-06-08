using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.QueryLoki;

public class QueryLokiQueryHandler(
    IBaseResourceRepository baseResourceRepository,
    IObservabilityClient observabilityClient) : IRequestHandler<QueryLokiQuery, PrometheusResultDto>
{
    public async Task<PrometheusResultDto?> Handle(QueryLokiQuery request, CancellationToken cancellationToken)
    {
        var resource = await baseResourceRepository.GetById(request.ResourceId);
        if (resource is null)
        {
            throw new EntityNotFoundException($"Resource with id {request.ResourceId} not found.");
        }

        var query = request.Query switch
        {
            PredefinedLokiQuery.ContainerLog =>
                $$"""{CLOUDY_RESOURCE_ID="{{resource.Id:N}}"}""",
            _ =>
                throw new ArgumentOutOfRangeException(nameof(request.Query))
        };
        return await observabilityClient.QueryLokiRange(
            query,
            request.Start,
            request.End,
            request.Step,
            request.Timeout,
            request.Limit);
    }
}
