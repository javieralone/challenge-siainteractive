namespace Challenge.Domain.Exceptions;

public class CategoryNotFoundException : Exception
{
    public CategoryNotFoundException(long id) : base($"Category with Id {id} not found")
    {
    }
}

