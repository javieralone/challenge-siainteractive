using Challenge.Commands.Products.Create;
using Challenge.Domain.Entities;
using Challenge.Domain.Exceptions;
using Challenge.Domain.Repositories;
using FluentAssertions;
using LanguageExt;
using NSubstitute;
using Xunit;

namespace Challenge.Commands.Tests.Products.Create;

public class CreateProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _handler = new CreateProductCommandHandler(_productRepository);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldCreateProductSuccessfully()
    {
        // Arrange
        var productName = "New Product";
        var productDescription = "Product Description";
        var request = new CreateProductCommandRequest(productName, productDescription, null);
        var expectedId = 1L;

        _productRepository.Validate(productName).Returns(Option<Product>.None);
        _productRepository
            .Save(Arg.Any<Product>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo => callInfo.Arg<Product>().Id = expectedId);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(expectedId);
        await _productRepository.Received(1).Save(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_WithImage_ShouldCreateProductWithImage()
    {
        // Arrange
        var productName = "New Product";
        var productDescription = "Product Description";
        var imageUrl = "https://example.com/image.jpg";
        var request = new CreateProductCommandRequest(productName, productDescription, imageUrl);
        var expectedId = 1L;

        _productRepository.Validate(productName).Returns(Option<Product>.None);
        _productRepository
            .Save(Arg.Any<Product>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo => callInfo.Arg<Product>().Id = expectedId);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(expectedId);
        await _productRepository.Received(1).Save(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenProductNameAlreadyExists_ShouldThrowDuplicatedProductException()
    {
        // Arrange
        var productName = "Existing Product";
        var productDescription = "Product Description";
        var request = new CreateProductCommandRequest(productName, productDescription, null);

        var existingProduct = Product.Create(productName, productDescription);
        existingProduct.Id = 1L;

        _productRepository.Validate(productName).Returns(Option<Product>.Some(existingProduct));

        // Act
        var act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DuplicatedProductException>()
            .WithMessage($"Product {productName} already exists");
        await _productRepository.DidNotReceive().Save(Arg.Any<Product>());
    }
}

