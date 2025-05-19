using AutoMapper;
using Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;
using Kjkardum.CloudyBack.Domain.Entities;

namespace Kjkardum.CloudyBack.Application.Mappings;

public class AuditLogEntryProfile: Profile
{
    public AuditLogEntryProfile()
    {
        CreateMap<AuditLogEntry, AuditLogDto>();
    }
}
