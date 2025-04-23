using ApiExceptions.Exceptions;
using AutoMapper;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Queries.GetById;

public class GetSinglePostgresServerResourceQueryHandler(
    IPostgresServerResourceRepository postgresServerResourceRepository,
    IMapper mapper) :
        IRequestHandler<GetSinglePostgresServerResourceQuery, PostgresServerResourceDto>
{
    public async Task<PostgresServerResourceDto> Handle(
        GetSinglePostgresServerResourceQuery request,
        CancellationToken cancellationToken)
    {
        var postgresServerResource = await postgresServerResourceRepository.GetById(request.Id);
        if (postgresServerResource is null)
        {
            throw new EntityNotFoundException($"Postgres server with id {request.Id} not found.");
        }
        return mapper.Map<PostgresServerResourceDto>(postgresServerResource);
    }
}
