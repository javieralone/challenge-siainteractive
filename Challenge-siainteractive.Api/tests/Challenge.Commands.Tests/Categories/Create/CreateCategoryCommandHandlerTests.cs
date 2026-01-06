using Challenge.Commands.Categories.Create;
using Challenge.Domain.Entities;
using Challenge.Domain.Exceptions;
using Challenge.Domain.Repositories;
using FluentAssertions;
using LanguageExt;
using NSubstitute;
using Xunit;

namespace Challenge.Commands.Tests.Categories.Create;

public class CreateCategoryCommandHandlerTests
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _handler = new CreateCategoryCommandHandler(_categoryRepository);
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ShouldCreateCategorySuccessfully()
    {
        // Arrange
        var categoryName = "New Category";
        var request = new CreateCategoryCommandRequest(categoryName);
        var expectedId = 1L;

        _categoryRepository.Validate(categoryName).Returns(Option<Category>.None);
        _categoryRepository
            .Save(Arg.Any<Category>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo => callInfo.Arg<Category>().Id = expectedId);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(expectedId);
        await _categoryRepository.Received(1).Save(Arg.Any<Category>());
    }

    [Fact]
    public async Task Handle_WhenCategoryNameAlreadyExists_ShouldThrowDuplicatedCategoryException()
    {
        // Arrange
        var categoryName = "Existing Category";
        var request = new CreateCategoryCommandRequest(categoryName);

        var existingCategory = Category.Create(categoryName);
        existingCategory.Id = 1L;

        _categoryRepository.Validate(categoryName).Returns(Option<Category>.Some(existingCategory));

        // Act
        var act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DuplicatedCategoryException>()
            .WithMessage($"Category {categoryName} already exists");
        await _categoryRepository.DidNotReceive().Save(Arg.Any<Category>());
    }
}

