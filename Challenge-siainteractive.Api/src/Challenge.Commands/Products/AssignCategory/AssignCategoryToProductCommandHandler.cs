using Challenge.Domain.Entities;
using Challenge.Domain.Exceptions;
using Challenge.Domain.Repositories;
using LanguageExt;
using MediatR;

namespace Challenge.Commands.Products.AssignCategory;

public class AssignCategoryToProductCommandHandler : IRequestHandler<AssignCategoryToProductCommandRequest, AssignCategoryToProductCommandResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductCategoryRepository _productCategoryRepository;

    public AssignCategoryToProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IProductCategoryRepository productCategoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _productCategoryRepository = productCategoryRepository;
    }

    public async Task<AssignCategoryToProductCommandResponse> Handle(AssignCategoryToProductCommandRequest request, CancellationToken cancellationToken)
    {
        var productOption = await _productRepository.Get(request.ProductId);
        productOption.IfNone(() => throw new ProductNotFoundException(request.ProductId));

        var categoryOption = await _categoryRepository.Get(request.CategoryId);
        categoryOption.IfNone(() => throw new CategoryNotFoundException(request.CategoryId));

        var existingRelation = await _productCategoryRepository.GetByProductAndCategory(request.ProductId, request.CategoryId);
        existingRelation.IfSome(_ => throw new DuplicatedProductCategoryException(request.ProductId, request.CategoryId));

        var productCategory = new ProductCategory(request.ProductId, request.CategoryId);
        await _productCategoryRepository.Save(productCategory);

        return new AssignCategoryToProductCommandResponse(request.ProductId, request.CategoryId);
    }
}

