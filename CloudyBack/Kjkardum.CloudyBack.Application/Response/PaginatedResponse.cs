namespace Kjkardum.CloudyBack.Application.Response;

public class PaginatedResponse<T> where T: class
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public required IEnumerable<T> Data { get; set; }
}
