using Challenge.Domain.Exceptions;
using Challenge.Domain.Repositories;
using MediatR;

namespace Challenge.Commands.Categories.Update;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommandRequest, UpdateCategoryCommandResponse>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<UpdateCategoryCommandResponse> Handle(UpdateCategoryCommandRequest request, CancellationToken cancellationToken)
    {
        var categoryOption = await _categoryRepository.Get(request.Id);

        var category = categoryOption.Match(
            Some: cat => cat,
            None: () => throw new CategoryNotFoundException(request.Id)
        );

        var existingCategoryOption = await _categoryRepository.Validate(request.Name);
        existingCategoryOption.IfSome(existingCategory =>
        {
            if (existingCategory.Id != request.Id)
            {
                throw new DuplicatedCategoryException(request.Name);
            }
        });

        category.Update(request.Name);
        await _categoryRepository.Save(category);

        return new UpdateCategoryCommandResponse(request.Id);
    }
}

