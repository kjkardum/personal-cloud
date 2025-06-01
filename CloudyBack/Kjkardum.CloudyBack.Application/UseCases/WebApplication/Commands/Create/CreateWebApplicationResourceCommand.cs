using Kjkardum.CloudyBack.Application.UseCases.WebApplication.Dto;
using Kjkardum.CloudyBack.Domain.Entities;
using Kjkardum.CloudyBack.Domain.Enums;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.WebApplication.Commands.Create;

public class CreateWebApplicationResourceCommand: IRequest<WebApplicationResourceDto>
{
    public string WebApplicationName { get; set; }
    public Guid ResourceGroupId { get; set; }
    public string SourcePath { get; set; }
    public WebApplicationSourceType SourceType { get; set; }
}
