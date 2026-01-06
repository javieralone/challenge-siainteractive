namespace Challenge.Queries.Common.Models;

public class PaginatedDataResponse<T>
{
    public int? PageNumber { get; set; }
    public int? RecordsPerPage { get; set; }
    public long TotalRecords { get; set; }
    public IList<T> Results { get; set; }
}
