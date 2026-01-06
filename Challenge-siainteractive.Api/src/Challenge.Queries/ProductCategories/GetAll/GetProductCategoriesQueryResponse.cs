using Challenge.Queries.ProductCategories.Models;
using Challenge.Queries.Common.Models;

namespace Challenge.Queries.ProductCategories.GetAll;

public class GetProductCategoriesQueryResponse
{
    public PaginatedDataResponse<ProductCategoriesDto> Result { get; set; }
}

