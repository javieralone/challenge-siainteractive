using Challenge.Domain.Entities;
using LanguageExt;

namespace Challenge.Domain.Repositories;

public interface IProductRepository
{
    Task<Option<Product>> Get(long productId);
    Task Save(Product product);
    Task<Option<Product>> Validate(string name);
}

