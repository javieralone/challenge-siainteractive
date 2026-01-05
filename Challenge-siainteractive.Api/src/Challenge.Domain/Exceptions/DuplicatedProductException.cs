namespace Challenge.Domain.Exceptions;

public class DuplicatedProductException : Exception
{
    public DuplicatedProductException(string name) : base($"Product {name} already exists")
    {
    }
}

