using Challenge.Commands.Products.Update;
using Challenge.Domain.Entities;
using Challenge.Domain.Exceptions;
using Challenge.Domain.Repositories;
using FluentAssertions;
using LanguageExt;
using NSubstitute;
using Xunit;

namespace Challenge.Commands.Tests.Products.Update;

public class UpdateProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly UpdateProductCommandHandler _handler;

    public UpdateProductCommandHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _handler = new UpdateProductCommandHandler(_productRepository);
    }

    [Fact]
    public async Task Handle_WhenProductExists_ShouldUpdateProductSuccessfully()
    {
        // Arrange
        var productId = 1L;
        var existingName = "Original Product";
        var existingDescription = "Original Description";
        var newName = "Updated Product";
        var newDescription = "Updated Description";
        var request = new UpdateProductCommandRequest(productId, newName, newDescription, null);

        var existingProduct = Product.Create(existingName, existingDescription);
        existingProduct.Id = productId;

        _productRepository.Get(productId).Returns(Option<Product>.Some(existingProduct));
        _productRepository.Validate(newName).Returns(Option<Product>.None);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(productId);
        existingProduct.Name.Should().Be(newName);
        existingProduct.Description.Should().Be(newDescription);
        await _productRepository.Received(1).Save(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenProductExists_WithImage_ShouldUpdateProductWithImage()
    {
        // Arrange
        var productId = 1L;
        var existingName = "Original Product";
        var existingDescription = "Original Description";
        var newName = "Updated Product";
        var newDescription = "Updated Description";
        var imageUrl = "https://example.com/image.jpg";
        var request = new UpdateProductCommandRequest(productId, newName, newDescription, imageUrl);

        var existingProduct = Product.Create(existingName, existingDescription);
        existingProduct.Id = productId;

        _productRepository.Get(productId).Returns(Option<Product>.Some(existingProduct));
        _productRepository.Validate(newName).Returns(Option<Product>.None);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(productId);
        existingProduct.Name.Should().Be(newName);
        existingProduct.Description.Should().Be(newDescription);
        existingProduct.Image.Should().NotBeNull();
        existingProduct.Image!.Value.Should().Be(imageUrl);
        await _productRepository.Received(1).Save(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldThrowProductNotFoundException()
    {
        // Arrange
        var productId = 999L;
        var newName = "New Product";
        var newDescription = "New Description";
        var request = new UpdateProductCommandRequest(productId, newName, newDescription, null);

        _productRepository.Get(productId).Returns(Option<Product>.None);

        // Act
        var act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ProductNotFoundException>()
            .WithMessage($"Product with Id {productId} not found");
        await _productRepository.DidNotReceive().Save(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenNameAlreadyExistsInAnotherProduct_ShouldThrowDuplicatedProductException()
    {
        // Arrange
        var productId = 1L;
        var existingName = "Original Product";
        var existingDescription = "Original Description";
        var duplicateName = "Duplicate Product";
        var newDescription = "New Description";
        var request = new UpdateProductCommandRequest(productId, duplicateName, newDescription, null);

        var existingProduct = Product.Create(existingName, existingDescription);
        existingProduct.Id = productId;

        var duplicateProduct = Product.Create(duplicateName, "Other Description");
        duplicateProduct.Id = 2L;

        _productRepository.Get(productId).Returns(Option<Product>.Some(existingProduct));
        _productRepository.Validate(duplicateName).Returns(Option<Product>.Some(duplicateProduct));

        // Act
        var act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DuplicatedProductException>()
            .WithMessage($"Product {duplicateName} already exists");
        await _productRepository.DidNotReceive().Save(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_WhenUpdatingToSameName_ShouldUpdateSuccessfully()
    {
        // Arrange
        var productId = 1L;
        var sameName = "Original Product";
        var existingDescription = "Original Description";
        var updatedDescription = "Updated Description";
        var request = new UpdateProductCommandRequest(productId, sameName, updatedDescription, null);

        var existingProduct = Product.Create(sameName, existingDescription);
        existingProduct.Id = productId;

        _productRepository.Get(productId).Returns(Option<Product>.Some(existingProduct));
        _productRepository.Validate(sameName).Returns(Option<Product>.Some(existingProduct));

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(productId);
        existingProduct.Name.Should().Be(sameName);
        existingProduct.Description.Should().Be(updatedDescription);
        await _productRepository.Received(1).Save(Arg.Any<Product>());
    }
}

