
namespace Kjkardum.CloudyBack.Application.UseCases.User.Dto;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public DateTime LastLogin { get; set; }
    public DateTime CreatedAt  { get; set; }
}
