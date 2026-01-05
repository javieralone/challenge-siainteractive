using Challenge.Domain.Entities;
using Challenge.Domain.Exceptions;
using Challenge.Domain.Repositories;
using Challenge.Domain.ValueObjects;
using MediatR;

namespace Challenge.Commands.Products.Create;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommandRequest, CreateProductCommandResponse>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<CreateProductCommandResponse> Handle(CreateProductCommandRequest request, CancellationToken cancellationToken)
    {
        var existingProduct = await _productRepository.Validate(request.Name);

        existingProduct.IfSome(x => throw new DuplicatedProductException(request.Name));

        var product = Product.Create(request.Name, request.Description);

        if (!string.IsNullOrWhiteSpace(request.Image))
        {
            var image = new Image(request.Image);
            product.AssignImage(image);
        }

        await _productRepository.Save(product);

        return new CreateProductCommandResponse(product.Id);
    }
}

