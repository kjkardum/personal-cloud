using ApiExceptions.Exceptions;
using AutoMapper;
using Kjkardum.CloudyBack.Application.Repositories;
using Kjkardum.CloudyBack.Application.UseCases.WebApplication.Dto;
using MediatR;

namespace Kjkardum.CloudyBack.Application.UseCases.WebApplication.Queries.GetById;

public class GetWebApplicationResourceByIdQueryHandler(
    IWebApplicationResourceRepository webApplicationResourceRepository,
    IMapper mapper) :
        IRequestHandler<GetWebApplicationResourceByIdQuery, WebApplicationResourceDto>
{
    public async Task<WebApplicationResourceDto> Handle(
        GetWebApplicationResourceByIdQuery request,
        CancellationToken cancellationToken)
    {
        var webApplicationResource = await webApplicationResourceRepository.GetById(request.Id);
        if (webApplicationResource == null)
        {
            throw new EntityNotFoundException($"Web application resource with ID {request.Id} not found.");
        }

        return mapper.Map<WebApplicationResourceDto>(webApplicationResource);
    }
}
