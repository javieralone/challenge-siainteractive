using Challenge.Domain.Exceptions;
using Challenge.Domain.Repositories;
using MediatR;

namespace Challenge.Queries.Products.GetById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQueryRequest, GetProductByIdQueryResponse>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<GetProductByIdQueryResponse> Handle(GetProductByIdQueryRequest request, CancellationToken cancellationToken)
    {
        var productOption = await _productRepository.Get(request.Id);

        var product = productOption.Match(
            Some: prod => prod,
            None: () => throw new ProductNotFoundException(request.Id)
        );

        return new GetProductByIdQueryResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Image?.Value);
    }
}

