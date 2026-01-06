using Challenge.Queries.Products.Models;
using Challenge.Queries.Common.Models;
using MediatR;

namespace Challenge.Queries.Products.GetAll;

public record GetProductsQueryRequest : IRequest<GetProductsQueryResponse>
{
    public PaginationRequest? Pagination { get; set; }
    public OrderFieldRequest<ProductsOrderBy>? OrderBy { get; set; }
}

