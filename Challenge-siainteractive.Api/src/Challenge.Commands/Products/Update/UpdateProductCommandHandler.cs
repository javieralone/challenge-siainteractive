using Challenge.Domain.Exceptions;
using Challenge.Domain.Repositories;
using Challenge.Domain.ValueObjects;
using LanguageExt;
using MediatR;

namespace Challenge.Commands.Products.Update;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommandRequest, UpdateProductCommandResponse>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<UpdateProductCommandResponse> Handle(UpdateProductCommandRequest request, CancellationToken cancellationToken)
    {
        var productOption = await _productRepository.Get(request.Id);

        var product = productOption.Match(
            Some: prod => prod,
            None: () => throw new ProductNotFoundException(request.Id)
        );

        var existingProductOption = await _productRepository.Validate(request.Name);
        existingProductOption.IfSome(existingProduct =>
        {
            if (existingProduct.Id != request.Id)
            {
                throw new DuplicatedProductException(request.Name);
            }
        });

        product.Update(request.Name, request.Description);

        if (!string.IsNullOrWhiteSpace(request.Image))
        {
            var image = new Image(request.Image);
            product.AssignImage(image);
        }

        await _productRepository.Save(product);

        return new UpdateProductCommandResponse(request.Id);
    }
}

