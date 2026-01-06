using Challenge.Domain.Entities;
using Challenge.Domain.Exceptions;
using Challenge.Domain.Repositories;
using MediatR;

namespace Challenge.Commands.Categories.Create;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommandRequest, CreateCategoryCommandResponse>
{
    private readonly ICategoryRepository _categoryRepository;


    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<CreateCategoryCommandResponse> Handle(CreateCategoryCommandRequest request, CancellationToken cancellationToken)
    {
        var existCategory = await _categoryRepository.Validate(request.Name);        

        existCategory.IfSome(x => throw new DuplicatedCategoryException(request.Name));

        var category = Category.Create(request.Name);

        await _categoryRepository.Save(category);

        return new CreateCategoryCommandResponse(category.Id);
    }
}
