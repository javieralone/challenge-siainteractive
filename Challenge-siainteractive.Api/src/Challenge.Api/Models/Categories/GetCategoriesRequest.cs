using Challenge.Queries.Common.Models;

namespace Challenge.Api.Models.Categories;

public class GetCategoriesRequest
{
    public int PageNumber { get; set; } = 1;
    public int RecordsPerPage { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public OrderFieldQueryDirection SortDirection { get; set; } = OrderFieldQueryDirection.ASC;
}

