using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using MediatR;
using System.Text.Json.Serialization;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.QueryPrometheus;

public enum PredefinedPrometheusQuery
{
    PostgresProcessesCount = 1,
}

public class QueryPrometheusQuery: IRequest<PrometheusResultDto>
{
    [JsonIgnore] public Guid ResourceId { get; set; }
    public PredefinedPrometheusQuery Query { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string? Step { get; set; }
    public string? Timeout { get; set; }
    public int? Limit { get; set; }
}
