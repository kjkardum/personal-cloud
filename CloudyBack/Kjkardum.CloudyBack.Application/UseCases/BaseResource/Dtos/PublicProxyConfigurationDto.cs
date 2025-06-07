namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;

public class PublicProxyConfigurationDto
{
    public Guid Id { get; set; }
    public bool UseHttps { get; set; }
    public string Domain { get; set; }
    public int Port { get; set; }
}
