namespace Challenge.Domain.Exceptions;

public class DuplicatedCategoryException : Exception
{
    public DuplicatedCategoryException(string name) : base($"Category {name} already exists")
    {
    }
}
