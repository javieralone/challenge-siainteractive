namespace Challenge.Domain.Exceptions;

public class InvalidImageFileException : Exception
{
    public InvalidImageFileException(string message) : base(message)
    {
    }
}

