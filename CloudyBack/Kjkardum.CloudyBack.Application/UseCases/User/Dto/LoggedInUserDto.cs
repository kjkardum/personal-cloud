namespace Kjkardum.CloudyBack.Application.UseCases.User.Dto;

public class LoggedInUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
}
