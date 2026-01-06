using Challenge.Domain.Exceptions;
using Challenge.Domain.Repositories;
using MediatR;

namespace Challenge.Queries.Categories.GetById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQueryRequest, GetCategoryByIdQueryResponse>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<GetCategoryByIdQueryResponse> Handle(GetCategoryByIdQueryRequest request, CancellationToken cancellationToken)
    {
        var categoryOption = await _categoryRepository.Get(request.Id);

        var category = categoryOption.Match(
            Some: cat => cat,
            None: () => throw new CategoryNotFoundException(request.Id)
        );

        return new GetCategoryByIdQueryResponse(category.Id, category.Name);
    }
}

