using AutoMapper;
using Kjkardum.CloudyBack.Application.UseCases.Postgres.Dtos;
using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Mappings;

public class PostgresProfile: Profile
{
    public PostgresProfile()
    {
        CreateMap<PostgresServerResource, PostgresServerResourceDto>();
        CreateMap<PostgresDatabaseResource, PostgresDatabaseResourceDto>();
    }
}
