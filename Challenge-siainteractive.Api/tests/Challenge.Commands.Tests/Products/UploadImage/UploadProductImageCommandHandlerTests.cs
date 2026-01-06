using Challenge.Commands.Products.UploadImage;
using Challenge.Domain.Entities;
using Challenge.Domain.Exceptions;
using Challenge.Domain.Repositories;
using Challenge.Domain.Services;
using Challenge.Domain.ValueObjects;
using FluentAssertions;
using LanguageExt;
using NSubstitute;
using Xunit;

namespace Challenge.Commands.Tests.Products.UploadImage;

public class UploadProductImageCommandHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly UploadProductImageCommandHandler _handler;

    public UploadProductImageCommandHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _imageStorageService = Substitute.For<IImageStorageService>();
        _handler = new UploadProductImageCommandHandler(_productRepository, _imageStorageService);
    }

    [Fact]
    public async Task Handle_WhenProductExists_ShouldUploadImageSuccessfully()
    {
        // Arrange
        var productId = 1L;
        var fileName = "test.jpg";
        var contentType = "image/jpeg";
        var imageUrl = "/images/products/test123.jpg";
        var imageStream = new MemoryStream(new byte[] { 1, 2, 3 });

        var product = Product.Create("Test Product", "Description");
        product.Id = productId;

        _productRepository.Get(productId).Returns(Option<Product>.Some(product));
        _imageStorageService.SaveImageAsync(Arg.Any<Stream>(), fileName, contentType).Returns(imageUrl);
        _productRepository
            .Save(Arg.Any<Product>())
            .Returns(Task.CompletedTask);

        var request = new UploadProductImageCommandRequest(productId, imageStream, fileName, contentType);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.ProductId.Should().Be(productId);
        response.ImageUrl.Should().Be(imageUrl);
        product.Image.Should().NotBeNull();
        product.Image!.Value.Should().Be(imageUrl);
        await _imageStorageService.Received(1).SaveImageAsync(Arg.Any<Stream>(), fileName, contentType);
        await _productRepository.Received(1).Save(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenProductExists_WithExistingImage_ShouldReplaceImage()
    {
        // Arrange
        var productId = 1L;
        var fileName = "new-image.jpg";
        var contentType = "image/jpeg";
        var oldImageUrl = "/images/products/old.jpg";
        var newImageUrl = "/images/products/new123.jpg";
        var imageStream = new MemoryStream(new byte[] { 1, 2, 3 });

        var product = Product.Create("Test Product", "Description");
        product.Id = productId;
        product.AssignImage(new Image(oldImageUrl));

        _productRepository.Get(productId).Returns(Option<Product>.Some(product));
        _imageStorageService.DeleteImageAsync(oldImageUrl).Returns(true);
        _imageStorageService.SaveImageAsync(Arg.Any<Stream>(), fileName, contentType).Returns(newImageUrl);
        _productRepository
            .Save(Arg.Any<Product>())
            .Returns(Task.CompletedTask);

        var request = new UploadProductImageCommandRequest(productId, imageStream, fileName, contentType);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.ImageUrl.Should().Be(newImageUrl);
        product.Image!.Value.Should().Be(newImageUrl);
        await _imageStorageService.Received(1).DeleteImageAsync(oldImageUrl);
        await _imageStorageService.Received(1).SaveImageAsync(Arg.Any<Stream>(), fileName, contentType);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldThrowProductNotFoundException()
    {
        // Arrange
        var productId = 999L;
        var fileName = "test.jpg";
        var contentType = "image/jpeg";
        var imageStream = new MemoryStream(new byte[] { 1, 2, 3 });

        _productRepository.Get(productId).Returns(Option<Product>.None);

        var request = new UploadProductImageCommandRequest(productId, imageStream, fileName, contentType);

        // Act
        var act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ProductNotFoundException>()
            .WithMessage($"Product with Id {productId} not found");
        await _imageStorageService.DidNotReceive().SaveImageAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>());
        await _productRepository.DidNotReceive().Save(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenFileSizeExceedsLimit_ShouldThrowInvalidImageFileException()
    {
        // Arrange
        var productId = 1L;
        var fileName = "large.jpg";
        var contentType = "image/jpeg";
        var largeImageStream = new MemoryStream(new byte[6 * 1024 * 1024]); // 6MB

        var product = Product.Create("Test Product", "Description");
        product.Id = productId;

        _productRepository.Get(productId).Returns(Option<Product>.Some(product));
        _imageStorageService
            .SaveImageAsync(Arg.Any<Stream>(), fileName, contentType)
            .Returns<Task<string>>(x => throw new InvalidImageFileException("File size exceeds maximum allowed size of 5MB"));

        var request = new UploadProductImageCommandRequest(productId, largeImageStream, fileName, contentType);

        // Act
        var act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidImageFileException>();
        await _productRepository.DidNotReceive().Save(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenFileTypeNotAllowed_ShouldThrowInvalidImageFileException()
    {
        // Arrange
        var productId = 1L;
        var fileName = "document.pdf";
        var contentType = "application/pdf";
        var imageStream = new MemoryStream(new byte[] { 1, 2, 3 });

        var product = Product.Create("Test Product", "Description");
        product.Id = productId;

        _productRepository.Get(productId).Returns(Option<Product>.Some(product));
        _imageStorageService
            .SaveImageAsync(Arg.Any<Stream>(), fileName, contentType)
            .Returns<Task<string>>(x => throw new InvalidImageFileException("File type not allowed"));

        var request = new UploadProductImageCommandRequest(productId, imageStream, fileName, contentType);

        // Act
        var act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidImageFileException>();
        await _productRepository.DidNotReceive().Save(Arg.Any<Product>());
    }
}

