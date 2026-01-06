using Challenge.Commands.Products.RemoveCategory;
using Challenge.Domain.Entities;
using Challenge.Domain.Exceptions;
using Challenge.Domain.Repositories;
using FluentAssertions;
using LanguageExt;
using NSubstitute;
using Xunit;

namespace Challenge.Commands.Tests.Products.RemoveCategory;

public class RemoveCategoryFromProductCommandHandlerTests
{
    private readonly IProductCategoryRepository _productCategoryRepository;
    private readonly RemoveCategoryFromProductCommandHandler _handler;

    public RemoveCategoryFromProductCommandHandlerTests()
    {
        _productCategoryRepository = Substitute.For<IProductCategoryRepository>();
        _handler = new RemoveCategoryFromProductCommandHandler(_productCategoryRepository);
    }

    [Fact]
    public async Task Handle_WhenRelationExists_ShouldRemoveCategorySuccessfully()
    {
        // Arrange
        var productId = 1L;
        var categoryId = 2L;
        var request = new RemoveCategoryFromProductCommandRequest(productId, categoryId);

        var productCategory = new ProductCategory(productId, categoryId);
        productCategory.Id = 1L;

        _productCategoryRepository.GetByProductAndCategory(productId, categoryId).Returns(Option<ProductCategory>.Some(productCategory));

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.ProductId.Should().Be(productId);
        response.CategoryId.Should().Be(categoryId);
        await _productCategoryRepository.Received(1).Delete(Arg.Any<ProductCategory>());
    }

    [Fact]
    public async Task Handle_WhenRelationDoesNotExist_ShouldThrowProductCategoryNotFoundException()
    {
        // Arrange
        var productId = 1L;
        var categoryId = 2L;
        var request = new RemoveCategoryFromProductCommandRequest(productId, categoryId);

        _productCategoryRepository.GetByProductAndCategory(productId, categoryId).Returns(Option<ProductCategory>.None);

        // Act
        var act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ProductCategoryNotFoundException>()
            .WithMessage($"Product {productId} is not assigned to Category {categoryId}");
        await _productCategoryRepository.DidNotReceive().Delete(Arg.Any<ProductCategory>());
    }
}

