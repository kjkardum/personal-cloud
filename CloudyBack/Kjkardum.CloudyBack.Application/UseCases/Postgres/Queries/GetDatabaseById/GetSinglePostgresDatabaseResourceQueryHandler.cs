using ApiExceptions.Exceptions;
using AutoMapper;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.Postgres.Queries.GetDatabaseById;

public class GetSinglePostgresDatabaseResourceQueryHandler(
    IPostgresDatabaseResourceRepository postgresDatabaseResourceRepository,
    IMapper mapper) : IRequestHandler<GetSinglePostgresDatabaseResourceQuery, PostgresDatabaseResourceDto>
{
    public async Task<PostgresDatabaseResourceDto> Handle(
        GetSinglePostgresDatabaseResourceQuery request,
        CancellationToken cancellationToken)
    {
        var postgresDatabaseResource = await postgresDatabaseResourceRepository.GetById(request.Id);
        if (postgresDatabaseResource is null)
        {
            throw new EntityNotFoundException($"Postgres database with id {request.Id} not found.");
        }
        return mapper.Map<PostgresDatabaseResourceDto>(postgresDatabaseResource);
    }
}
