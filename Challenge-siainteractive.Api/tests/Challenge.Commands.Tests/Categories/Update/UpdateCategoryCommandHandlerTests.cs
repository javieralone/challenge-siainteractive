using Challenge.Commands.Categories.Update;
using Challenge.Domain.Entities;
using Challenge.Domain.Exceptions;
using Challenge.Domain.Repositories;
using FluentAssertions;
using LanguageExt;
using NSubstitute;
using Xunit;

namespace Challenge.Commands.Tests.Categories.Update;

public class UpdateCategoryCommandHandlerTests
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly UpdateCategoryCommandHandler _handler;

    public UpdateCategoryCommandHandlerTests()
    {
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _handler = new UpdateCategoryCommandHandler(_categoryRepository);
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_ShouldUpdateCategorySuccessfully()
    {
        // Arrange
        var categoryId = 1L;
        var existingName = "Original Category";
        var newName = "Updated Category";
        var request = new UpdateCategoryCommandRequest(categoryId, newName);

        var existingCategory = Category.Create(existingName);
        existingCategory.Id = categoryId;

        _categoryRepository.Get(categoryId).Returns(Option<Category>.Some(existingCategory));
        _categoryRepository.Validate(newName).Returns(Option<Category>.None);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(categoryId);
        existingCategory.Name.Should().Be(newName);
        await _categoryRepository.Received(1).Save(Arg.Any<Category>());
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ShouldThrowCategoryNotFoundException()
    {
        // Arrange
        var categoryId = 999L;
        var newName = "New Category";
        var request = new UpdateCategoryCommandRequest(categoryId, newName);

        _categoryRepository.Get(categoryId).Returns(Option<Category>.None);

        // Act
        var act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<CategoryNotFoundException>()
            .WithMessage($"Category with Id {categoryId} not found");
        await _categoryRepository.DidNotReceive().Save(Arg.Any<Category>());
    }

    [Fact]
    public async Task Handle_WhenNameAlreadyExistsInAnotherCategory_ShouldThrowDuplicatedCategoryException()
    {
        // Arrange
        var categoryId = 1L;
        var existingName = "Original Category";
        var duplicateName = "Duplicate Category";
        var request = new UpdateCategoryCommandRequest(categoryId, duplicateName);

        var existingCategory = Category.Create(existingName);
        existingCategory.Id = categoryId;

        var duplicateCategory = Category.Create(duplicateName);
        duplicateCategory.Id = 2L;

        _categoryRepository.Get(categoryId).Returns(Option<Category>.Some(existingCategory));
        _categoryRepository.Validate(duplicateName).Returns(Option<Category>.Some(duplicateCategory));

        // Act
        var act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DuplicatedCategoryException>()
            .WithMessage($"Category {duplicateName} already exists");
        await _categoryRepository.DidNotReceive().Save(Arg.Any<Category>());
    }

    [Fact]
    public async Task Handle_WhenUpdatingToSameName_ShouldUpdateSuccessfully()
    {
        // Arrange
        var categoryId = 1L;
        var sameName = "Original Category";
        var request = new UpdateCategoryCommandRequest(categoryId, sameName);

        var existingCategory = Category.Create(sameName);
        existingCategory.Id = categoryId;

        _categoryRepository.Get(categoryId).Returns(Option<Category>.Some(existingCategory));
        _categoryRepository.Validate(sameName).Returns(Option<Category>.Some(existingCategory));

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(categoryId);
        existingCategory.Name.Should().Be(sameName);
        await _categoryRepository.Received(1).Save(Arg.Any<Category>());
    }
}

