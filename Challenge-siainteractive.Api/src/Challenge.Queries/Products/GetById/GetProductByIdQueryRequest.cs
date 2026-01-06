using MediatR;

namespace Challenge.Queries.Products.GetById;

public record GetProductByIdQueryRequest(long Id) : IRequest<GetProductByIdQueryResponse>;

