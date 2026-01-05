namespace Challenge.Domain.Services;

public interface IImageStorageService
{
    Task<string> SaveImageAsync(Stream imageStream, string fileName, string contentType);
    Task<bool> DeleteImageAsync(string imageUrl);
}

