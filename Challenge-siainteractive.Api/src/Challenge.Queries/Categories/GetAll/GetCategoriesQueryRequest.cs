using Challenge.Queries.Categories.Models;
using Challenge.Queries.Common.Models;
using MediatR;

namespace Challenge.Queries.Categories.GetAll;

public record GetCategoriesQueryRequest : IRequest<GetCategoriesQueryResponse>
{
    public PaginationRequest? Pagination { get; set; }
    public OrderFieldRequest<CategoriesOrderBy>? OrderBy { get; set; }
    public string? SearchTerm { get; set; }
}

