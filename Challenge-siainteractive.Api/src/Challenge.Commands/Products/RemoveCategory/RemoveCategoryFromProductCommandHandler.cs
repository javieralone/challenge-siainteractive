using Challenge.Domain.Exceptions;
using Challenge.Domain.Repositories;
using MediatR;

namespace Challenge.Commands.Products.RemoveCategory;

public class RemoveCategoryFromProductCommandHandler : IRequestHandler<RemoveCategoryFromProductCommandRequest, RemoveCategoryFromProductCommandResponse>
{
    private readonly IProductCategoryRepository _productCategoryRepository;

    public RemoveCategoryFromProductCommandHandler(IProductCategoryRepository productCategoryRepository)
    {
        _productCategoryRepository = productCategoryRepository;
    }

    public async Task<RemoveCategoryFromProductCommandResponse> Handle(RemoveCategoryFromProductCommandRequest request, CancellationToken cancellationToken)
    {
        var productCategoryOption = await _productCategoryRepository.GetByProductAndCategory(request.ProductId, request.CategoryId);
        
        var productCategory = productCategoryOption.Match(
            Some: pc => pc,
            None: () => throw new ProductCategoryNotFoundException(request.ProductId, request.CategoryId)
        );

        await _productCategoryRepository.Delete(productCategory);

        return new RemoveCategoryFromProductCommandResponse(request.ProductId, request.CategoryId);
    }
}

