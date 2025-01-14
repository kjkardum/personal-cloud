namespace Kjkardum.CloudyBack.Api.ProblemDetails;

internal record ErrorInfoDto
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = null!;
}
