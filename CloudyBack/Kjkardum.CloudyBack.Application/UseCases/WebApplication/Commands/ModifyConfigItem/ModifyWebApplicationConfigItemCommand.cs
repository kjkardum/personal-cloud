using MediatR;
using System.Text.Json.Serialization;

namespace Kjkardum.CloudyBack.Application.UseCases.WebApplication.Commands.ModifyConfigItem;

public class ModifyWebApplicationConfigItemCommand: IRequest
{
    [JsonIgnore] public Guid WebApplicationId { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}
