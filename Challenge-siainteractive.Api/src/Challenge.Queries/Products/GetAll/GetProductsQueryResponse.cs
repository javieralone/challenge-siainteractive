using Challenge.Queries.Products.Models;
using Challenge.Queries.Common.Models;

namespace Challenge.Queries.Products.GetAll;

public class GetProductsQueryResponse
{
    public PaginatedDataResponse<ProductsDto> Result { get; set; }
}

