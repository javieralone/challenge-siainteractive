using Challenge.Domain.Entities;
using Challenge.Domain.Repositories;
using Challenge.Infrastructure.Data.Persistence;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Infrastructure.Data.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly ChallengeDBContext _dbContext;

    public ProductCategoryRepository(ChallengeDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Option<ProductCategory>> GetByProductAndCategory(long productId, long categoryId)
    {
        return await _dbContext.ProductCategories
            .FirstOrDefaultAsync(x => x.ProductId == productId && x.CategoryId == categoryId);
    }

    public async Task Save(ProductCategory productCategory)
    {
        _dbContext.ProductCategories.Attach(productCategory);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(ProductCategory productCategory)
    {
        _dbContext.ProductCategories.Remove(productCategory);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<ProductCategory>> GetByProductId(long productId)
    {
        return await _dbContext.ProductCategories
            .Where(x => x.ProductId == productId)
            .ToListAsync();
    }
}

