using Kjkardum.CloudyBack.Application.UseCases.WebApplication.Dto;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.WebApplication.Queries.GetById;

public class GetWebApplicationResourceByIdQuery: IRequest<WebApplicationResourceDto>
{
    public Guid Id { get; set; }
}
