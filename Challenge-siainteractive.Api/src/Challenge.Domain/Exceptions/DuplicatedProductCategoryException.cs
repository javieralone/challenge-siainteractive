namespace Challenge.Domain.Exceptions;

public class DuplicatedProductCategoryException : Exception
{
    public DuplicatedProductCategoryException(long productId, long categoryId) 
        : base($"Product {productId} is already assigned to Category {categoryId}")
    {
    }
}

