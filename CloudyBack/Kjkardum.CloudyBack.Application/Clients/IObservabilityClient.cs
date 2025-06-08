using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;

namespace Kjkardum.CloudyBack.Application.Clients;

public interface IObservabilityClient
{
    Task AttachCollector(Guid id, string jobName);
    Task<PrometheusResultDto?> QueryPrometheusRange(string query, DateTime requestStart, DateTime requestEnd, string? requestStep, string? requestTimeout, int? requestLimit);
    Task<PrometheusResultDto?> QueryLokiRange(string query, DateTime requestStart, DateTime requestEnd, string? requestStep, string? requestTimeout, int? requestLimit);
    Task EnsureCreated();
}
