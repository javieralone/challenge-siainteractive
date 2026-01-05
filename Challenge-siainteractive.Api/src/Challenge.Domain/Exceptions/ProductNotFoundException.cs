namespace Challenge.Domain.Exceptions;

public class ProductNotFoundException : Exception
{
    public ProductNotFoundException(long id) : base($"Product with Id {id} not found")
    {
    }
}

