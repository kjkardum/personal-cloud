namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;

public class GrafanaConfigurationDto
{
    public bool Created { get; set; }
    public string Host { get; set; } = string.Empty;
    public bool UseHttps { get; set; }
}
