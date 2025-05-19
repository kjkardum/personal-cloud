using ApiExceptions.Exceptions;
using Kjkardum.CloudyBack.Application.Clients;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Queries.QueryPrometheus;

public class QueryPrometheusQueryHandler(
    IBaseResourceRepository baseResourceRepository,
    IPrometheusClient prometheusClient) : IRequestHandler<QueryPrometheusQuery, PrometheusResultDto>
{
    public async Task<PrometheusResultDto?> Handle(QueryPrometheusQuery request, CancellationToken cancellationToken)
    {
        var resource = await baseResourceRepository.GetById(request.ResourceId);
        if (resource is null)
        {
            throw new EntityNotFoundException($"Resource with id {request.ResourceId} not found.");
        }

        switch (request.Query)
        {
            case PredefinedPrometheusQuery.PostgresProcessesCount:
                return await prometheusClient.QueryRange(
                    $$"""postgresql_backends{job="{{resource.Name}}"}""",
                    request.Start,
                    request.End,
                    request.Step,
                    request.Timeout,
                    request.Limit);
            default:
                throw new ArgumentOutOfRangeException(nameof(request.Query));
        }
    }
}
