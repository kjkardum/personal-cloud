namespace Kjkardum.CloudyBack.Application.Response;

public class PaginatedResponse<T> where T: class
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public IEnumerable<T> Data { get; set; }

    public PaginatedResponse(int pageNumber, int pageSize, int totalRecords, IEnumerable<T> data)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalRecords = totalRecords;
        Data = data;
    }
}
