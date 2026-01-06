using Challenge.Queries.ProductCategories.Models;
using Challenge.Queries.Common.Models;
using MediatR;

namespace Challenge.Queries.ProductCategories.GetAll;

public record GetProductCategoriesQueryRequest : IRequest<GetProductCategoriesQueryResponse>
{
    public long? ProductId { get; set; }
    public long? CategoryId { get; set; }
    public PaginationRequest? Pagination { get; set; }
    public OrderFieldRequest<ProductCategoriesOrderBy>? OrderBy { get; set; }
}

