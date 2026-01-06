using Challenge.Domain.Exceptions;
using Challenge.Domain.Repositories;
using Challenge.Domain.Services;
using Challenge.Domain.ValueObjects;
using MediatR;

namespace Challenge.Commands.Products.UploadImage;

public class UploadProductImageCommandHandler : IRequestHandler<UploadProductImageCommandRequest, UploadProductImageCommandResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly IImageStorageService _imageStorageService;

    public UploadProductImageCommandHandler(
        IProductRepository productRepository,
        IImageStorageService imageStorageService)
    {
        _productRepository = productRepository;
        _imageStorageService = imageStorageService;
    }

    public async Task<UploadProductImageCommandResponse> Handle(UploadProductImageCommandRequest request, CancellationToken cancellationToken)
    {
        var productOption = await _productRepository.Get(request.ProductId);

        var product = productOption.Match(
            Some: prod => prod,
            None: () => throw new ProductNotFoundException(request.ProductId)
        );

        // Delete old image if exists
        if (product.Image != null)
        {
            await _imageStorageService.DeleteImageAsync(product.Image.Value);
        }

        // Save new image
        var imageUrl = await _imageStorageService.SaveImageAsync(
            request.ImageStream,
            request.FileName,
            request.ContentType);

        // Update product with new image
        var image = new Image(imageUrl);
        product.AssignImage(image);

        await _productRepository.Save(product);

        return new UploadProductImageCommandResponse(request.ProductId, imageUrl);
    }
}

