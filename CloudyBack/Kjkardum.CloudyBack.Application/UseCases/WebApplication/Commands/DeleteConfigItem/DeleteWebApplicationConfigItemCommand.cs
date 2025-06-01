using MediatR;
using System.Text.Json.Serialization;

namespace Kjkardum.CloudyBack.Application.UseCases.WebApplication.Commands.DeleteConfigItem;

public class DeleteWebApplicationConfigItemCommand: IRequest
{
    [JsonIgnore] public Guid WebApplicationId { get; set; }
    [JsonIgnore] public string Key { get; set; }
}
