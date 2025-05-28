using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.QueryPrometheus;

public class QueryPrometheusQueryHandler(
    IBaseResourceRepository baseResourceRepository,
    IGeneralContainerStatusClient generalContainerStatusClient,
    IObservabilityClient observabilityClient) : IRequestHandler<QueryPrometheusQuery, PrometheusResultDto>
{
    public async Task<PrometheusResultDto?> Handle(QueryPrometheusQuery request, CancellationToken cancellationToken)
    {
        var resource = await baseResourceRepository.GetById(request.ResourceId);
        if (resource is null)
        {
            throw new EntityNotFoundException($"Resource with id {request.ResourceId} not found.");
        }

        var container = await generalContainerStatusClient.GetContainerStateAsync(resource.Id);

        var query = request.Query switch
        {
            PredefinedPrometheusQuery.PostgresProcessesCount =>
                $$"""postgresql_backends{job="{{resource.Name}}"}""",
            PredefinedPrometheusQuery.PostgresEntriesReturned =>
                $$"""rate(postgresql_tup_returned_total{job="{{resource.Name}}"}[1m])""",
            PredefinedPrometheusQuery.PostgresEntriesInserted =>
                $$"""rate(postgresql_tup_inserted_total{job="{{resource.Name}}"}[1m])""",
            PredefinedPrometheusQuery.GeneralCPULoad =>
                $$"""rate(container_cpu_usage_seconds_total{id="/docker/{{container.ContainerId}}"}[1m])""",
            PredefinedPrometheusQuery.GeneralMemoryUsage =>
                $$"""container_memory_usage_bytes{id="/docker/{{container.ContainerId}}"}/1000/1000""",
            _ =>
                throw new ArgumentOutOfRangeException(nameof(request.Query))
        };
        return await observabilityClient.QueryPrometheusRange(
            query,
            request.Start,
            request.End,
            request.Step,
            request.Timeout,
            request.Limit);
    }
}
