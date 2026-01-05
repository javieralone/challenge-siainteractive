using Challenge.Domain.Abstractions;

namespace Challenge.Domain.Entities;

public class ProductCategory : EntityBase
{
    public long ProductId { get; internal set; }
    public long CategoryId { get; internal set; }

    public Product Product { get; internal set; } = null!;
    public Category Category { get; internal set; } = null!;

    private ProductCategory() { }

    public ProductCategory(long productId, long categoryId)
    {
        ProductId = productId;
        CategoryId = categoryId;
    }
}

