using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.WebApplication.Commands.Delete;

public class DeleteWebApplicationResourceCommand: IRequest
{
    public Guid Id { get; set; }
}
