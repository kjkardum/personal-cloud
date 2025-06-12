using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Configuration;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using Kjkardum.CloudyBack.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.QueryPrometheus;

public class QueryPrometheusQueryHandler(
    IBaseResourceRepository baseResourceRepository,
    IWebApplicationResourceRepository webApplicationResourceRepository,
    IGeneralContainerStatusClient generalContainerStatusClient,
    IOptions<AppConfiguration> appConfiguration,
    IObservabilityClient observabilityClient) : IRequestHandler<QueryPrometheusQuery, PrometheusResultDto>
{
    private readonly bool OnLinux = appConfiguration.Value.UnameO
        .Contains("linux", StringComparison.InvariantCultureIgnoreCase);
    public async Task<PrometheusResultDto?> Handle(QueryPrometheusQuery request, CancellationToken cancellationToken)
    {
        var resource = await baseResourceRepository.GetById(request.ResourceId);

        var container = await generalContainerStatusClient.GetContainerStateAsync(resource?.Id ?? Guid.Empty);
        var containerPath = GetContainerPath(container);
        var idParam = GetCadvisorIdParam(container);

        var query = request.Query switch
        {
            PredefinedPrometheusQuery.PostgresProcessesCount =>
                ThrowNullResource(resource) ?? $$"""postgresql_backends{job="{{resource!.Name}}"}""",
            PredefinedPrometheusQuery.PostgresEntriesReturned =>
                ThrowNullResource(resource)
                    ?? $$"""delta(postgresql_tup_returned_total{job="{{resource!.Name}}"}[1m])""",
            PredefinedPrometheusQuery.PostgresEntriesInserted =>
                ThrowNullResource(resource)
                    ?? $$"""delta(postgresql_tup_inserted_total{job="{{resource!.Name}}"}[1m])""",
            PredefinedPrometheusQuery.GeneralCPULoad =>
                $$"""delta(container_cpu_usage_seconds_total{{{idParam}}="{{containerPath}}"}[1m])""",
            PredefinedPrometheusQuery.GeneralMemoryUsage =>
                $$"""container_memory_usage_bytes{{{idParam}}="{{containerPath}}"}/1000/1000""",
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

    private string GetCadvisorIdParam(DockerContainer? container)
        => OnLinux && container != null ? "name" : "id";

    private string GetContainerPath(DockerContainer? container)
    {
        if (OnLinux)
        {
            return container == null ? "/" : container.ContainerName;
        }

        return "/docker" + (container == null ? string.Empty : $"/{container.ContainerId}");
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
