using Challenge.Queries.Categories.Models;
using Challenge.Queries.Common.Models;

namespace Challenge.Queries.Categories.GetAll;

public class GetCategoriesQueryResponse
{
    public PaginatedDataResponse<CategoriesDto> Result { get; set; }
}


