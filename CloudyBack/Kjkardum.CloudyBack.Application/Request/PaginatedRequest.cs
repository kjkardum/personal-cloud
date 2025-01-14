namespace Kjkardum.CloudyBack.Application.Request;

public record PaginatedRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? FilterBy { get; set; }
    public string? OrderBy { get; set; }
}
