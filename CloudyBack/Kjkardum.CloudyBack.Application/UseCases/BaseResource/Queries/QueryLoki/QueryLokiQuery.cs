using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using MediatR;
using System.Text.Json.Serialization;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.QueryLoki;

public enum PredefinedLokiQuery
{
    ContainerLog = 1
}

public class QueryLokiQuery: IRequest<PrometheusResultDto>
{
    [JsonIgnore] public Guid ResourceId { get; set; }
    public PredefinedLokiQuery Query { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string? Step { get; set; }
    public string? Timeout { get; set; }
    public int? Limit { get; set; }
}
