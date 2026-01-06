using Challenge.Domain.Entities;
using LanguageExt;

namespace Challenge.Domain.Repositories;

public interface IProductCategoryRepository
{
    Task<Option<ProductCategory>> GetByProductAndCategory(long productId, long categoryId);
    Task Save(ProductCategory productCategory);
    Task Delete(ProductCategory productCategory);
    Task<List<ProductCategory>> GetByProductId(long productId);
}

