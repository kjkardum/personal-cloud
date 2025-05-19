using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;

namespace Kjkardum.CloudyBack.Application.Clients;

public interface IPrometheusClient
{
    Task AttachCollector(Guid id, string jobName);
    Task<PrometheusResultDto?> QueryRange(string query, DateTime requestStart, DateTime requestEnd, string? requestStep, string? requestTimeout, int? requestLimit);
}
