namespace Kjkardum.CloudyBack.Application.UseCases.BaseResource.Dtos;

public class PrometheusResultDto
{
    public string status { get; set; } = string.Empty;
    public PrometheusDataDto data { get; set; } = new();
}

public class PrometheusDataDto
{
    public string resultType { get; set; } = string.Empty;
    public List<PrometheusResultItemDto> result { get; set; } = new();
}
public class PrometheusResultItemDto
{
    public Dictionary<string, string> metric { get; set; } = new();
    public List<List<object>> values { get; set; } = new();
}
