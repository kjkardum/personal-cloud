using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.QueryPrometheus;

public class QueryPrometheusQueryHandler(
    IBaseResourceRepository baseResourceRepository,
    IWebApplicationResourceRepository webApplicationResourceRepository,
    IGeneralContainerStatusClient generalContainerStatusClient,
    IObservabilityClient observabilityClient) : IRequestHandler<QueryPrometheusQuery, PrometheusResultDto>
{
    public async Task<PrometheusResultDto?> Handle(QueryPrometheusQuery request, CancellationToken cancellationToken)
    {
        var resource = await baseResourceRepository.GetById(request.ResourceId);

        var container = await generalContainerStatusClient.GetContainerStateAsync(resource?.Id ?? Guid.Empty);
        var containerPath = container == null ? string.Empty : $"/{container.ContainerId}";

        var query = request.Query switch
        {
            PredefinedPrometheusQuery.PostgresProcessesCount =>
                ThrowNullResource(resource) ?? $$"""postgresql_backends{job="{{resource!.Name}}"}""",
            PredefinedPrometheusQuery.PostgresEntriesReturned =>
                ThrowNullResource(resource) ?? $$"""delta(postgresql_tup_returned_total{job="{{resource!.Name}}"}[1m])""",
            PredefinedPrometheusQuery.PostgresEntriesInserted =>
                ThrowNullResource(resource) ?? $$"""delta(postgresql_tup_inserted_total{job="{{resource!.Name}}"}[1m])""",
            PredefinedPrometheusQuery.GeneralCPULoad =>
                $$"""delta(container_cpu_usage_seconds_total{id="/docker{{containerPath}}"}[1m])""",
            PredefinedPrometheusQuery.GeneralMemoryUsage =>
                $$"""container_memory_usage_bytes{id="/docker{{containerPath}}"}/1000/1000""",
            PredefinedPrometheusQuery.HttpRequestsCount =>
                await GetWebApplicationRequestCountQuery(resource?.Id),
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
    private static string? ThrowNullResource(Domain.Entities.BaseResource? resource)
    {
        if (resource == null)
        {
            throw new EntityNotFoundException("Resource not found.");
        }
        return null;
    }

    private async Task<string> GetWebApplicationRequestCountQuery(Guid? resourceId)
    {
        if (resourceId == null)
        {
            return "delta(caddy_http_requests_total[1m])";
        }
        var webApplication = await webApplicationResourceRepository.GetById(resourceId.Value);
        if (webApplication == null)
        {
            throw new EntityNotFoundException($"Web application with id {resourceId} not found.");
        }

        var hostsOfApp = string.Join(',', webApplication.PublicProxyConfigurations!.Select(t => t.Domain));
        return $$"""delta(caddy_http_requests_total{host=~"{{hostsOfApp}}"}[1m])""";
    }
}
