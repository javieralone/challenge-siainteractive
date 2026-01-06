using Challenge.Queries.Common.Models;

namespace Challenge.Api.Models.ProductCategories;

public class GetProductCategoriesRequest
{
    public long? ProductId { get; set; }
    public long? CategoryId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int RecordsPerPage { get; set; } = 10;
    public string? SortBy { get; set; }
    public OrderFieldQueryDirection SortDirection { get; set; } = OrderFieldQueryDirection.ASC;
}

