namespace Challenge.Queries.Common.Models;

public class PaginationRequest
{
    public int PageNumber { get; set; }

    public int RecordsPerPage { get; set; }

    public int SkipRecords => (PageNumber - 1) * RecordsPerPage;
}
