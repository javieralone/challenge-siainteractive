namespace Challenge.Domain.Exceptions;

public class ProductCategoryNotFoundException : Exception
{
    public ProductCategoryNotFoundException(long productId, long categoryId) 
        : base($"Product {productId} is not assigned to Category {categoryId}")
    {
    }
}

