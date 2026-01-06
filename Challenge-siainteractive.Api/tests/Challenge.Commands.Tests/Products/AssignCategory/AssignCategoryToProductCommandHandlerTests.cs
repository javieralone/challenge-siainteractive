using Challenge.Commands.Products.AssignCategory;
using Challenge.Domain.Entities;
using Challenge.Domain.Exceptions;
using Challenge.Domain.Repositories;
using FluentAssertions;
using LanguageExt;
using NSubstitute;
using Xunit;

namespace Challenge.Commands.Tests.Products.AssignCategory;

public class AssignCategoryToProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductCategoryRepository _productCategoryRepository;
    private readonly AssignCategoryToProductCommandHandler _handler;

    public AssignCategoryToProductCommandHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _productCategoryRepository = Substitute.For<IProductCategoryRepository>();
        _handler = new AssignCategoryToProductCommandHandler(_productRepository, _categoryRepository, _productCategoryRepository);
    }

    [Fact]
    public async Task Handle_WhenProductAndCategoryExist_ShouldAssignCategorySuccessfully()
    {
        // Arrange
        var productId = 1L;
        var categoryId = 2L;
        var request = new AssignCategoryToProductCommandRequest(productId, categoryId);

        var product = Product.Create("Test Product", "Description");
        product.Id = productId;

        var category = Category.Create("Test Category");
        category.Id = categoryId;

        _productRepository.Get(productId).Returns(Option<Product>.Some(product));
        _categoryRepository.Get(categoryId).Returns(Option<Category>.Some(category));
        _productCategoryRepository.GetByProductAndCategory(productId, categoryId).Returns(Option<ProductCategory>.None);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.ProductId.Should().Be(productId);
        response.CategoryId.Should().Be(categoryId);
        await _productCategoryRepository.Received(1).Save(Arg.Any<ProductCategory>());
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldThrowProductNotFoundException()
    {
        // Arrange
        var productId = 999L;
        var categoryId = 2L;
        var request = new AssignCategoryToProductCommandRequest(productId, categoryId);

        _productRepository.Get(productId).Returns(Option<Product>.None);

        // Act
        var act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ProductNotFoundException>()
            .WithMessage($"Product with Id {productId} not found");
        await _productCategoryRepository.DidNotReceive().Save(Arg.Any<ProductCategory>());
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ShouldThrowCategoryNotFoundException()
    {
        // Arrange
        var productId = 1L;
        var categoryId = 999L;
        var request = new AssignCategoryToProductCommandRequest(productId, categoryId);

        var product = Product.Create("Test Product", "Description");
        product.Id = productId;

        _productRepository.Get(productId).Returns(Option<Product>.Some(product));
        _categoryRepository.Get(categoryId).Returns(Option<Category>.None);

        // Act
        var act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<CategoryNotFoundException>()
            .WithMessage($"Category with Id {categoryId} not found");
        await _productCategoryRepository.DidNotReceive().Save(Arg.Any<ProductCategory>());
    }

    [Fact]
    public async Task Handle_WhenRelationAlreadyExists_ShouldThrowDuplicatedProductCategoryException()
    {
        // Arrange
        var productId = 1L;
        var categoryId = 2L;
        var request = new AssignCategoryToProductCommandRequest(productId, categoryId);

        var product = Product.Create("Test Product", "Description");
        product.Id = productId;

        var category = Category.Create("Test Category");
        category.Id = categoryId;

        var existingRelation = new ProductCategory(productId, categoryId);
        existingRelation.Id = 1L;

        _productRepository.Get(productId).Returns(Option<Product>.Some(product));
        _categoryRepository.Get(categoryId).Returns(Option<Category>.Some(category));
        _productCategoryRepository.GetByProductAndCategory(productId, categoryId).Returns(Option<ProductCategory>.Some(existingRelation));

        // Act
        var act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DuplicatedProductCategoryException>()
            .WithMessage($"Product {productId} is already assigned to Category {categoryId}");
        await _productCategoryRepository.DidNotReceive().Save(Arg.Any<ProductCategory>());
    }
}

