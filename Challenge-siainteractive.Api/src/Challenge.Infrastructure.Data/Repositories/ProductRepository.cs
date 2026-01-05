using Challenge.Domain.Entities;
using Challenge.Domain.Repositories;
using Challenge.Infrastructure.Data.Persistence;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Infrastructure.Data.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ChallengeDBContext _dbContext;

    public ProductRepository(ChallengeDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Option<Product>> Get(long productId)
    {
        return await _dbContext.Products.FirstOrDefaultAsync(x => x.Id.Equals(productId));
    }

    public async Task Save(Product product)
    {
        _dbContext.Products.Attach(product);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Option<Product>> Validate(string name)
    {
        return await _dbContext.Products.FirstOrDefaultAsync(x => x.Name.Equals(name));
    }
}

