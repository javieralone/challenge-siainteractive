using Challenge.Domain.Exceptions;
using Challenge.Domain.Services;
using Microsoft.AspNetCore.Hosting;

namespace Challenge.Infrastructure.CrossCutting.Storage;

public class ImageStorageService : IImageStorageService
{
    private readonly IWebHostEnvironment _environment;
    private const string ImagesFolder = "images/products";
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public ImageStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveImageAsync(Stream imageStream, string fileName, string contentType)
    {
        // Copy stream to memory stream to read length and allow multiple reads
        using var memoryStream = new MemoryStream();
        await imageStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        ValidateFile(fileName, contentType, memoryStream.Length);

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var imagesPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, ImagesFolder);
        
        if (!Directory.Exists(imagesPath))
        {
            Directory.CreateDirectory(imagesPath);
        }

        var filePath = Path.Combine(imagesPath, uniqueFileName);
        
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(fileStream);
        }

        return $"/{ImagesFolder}/{uniqueFileName}";
    }

    public Task<bool> DeleteImageAsync(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return Task.FromResult(false);
        }

        try
        {
            var fileName = Path.GetFileName(imageUrl);
            var imagesPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, ImagesFolder);
            var filePath = Path.Combine(imagesPath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private void ValidateFile(string fileName, string contentType, long fileSize)
    {
        if (fileSize > MaxFileSize)
        {
            throw new InvalidImageFileException($"File size exceeds maximum allowed size of {MaxFileSize / (1024 * 1024)}MB");
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            throw new InvalidImageFileException($"File type not allowed. Allowed types: {string.Join(", ", _allowedExtensions)}");
        }

        var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
        if (!allowedContentTypes.Contains(contentType.ToLowerInvariant()))
        {
            throw new InvalidImageFileException($"Content type not allowed. Allowed types: {string.Join(", ", allowedContentTypes)}");
        }
    }
}

