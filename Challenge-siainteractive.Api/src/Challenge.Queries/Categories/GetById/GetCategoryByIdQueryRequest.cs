using MediatR;

namespace Challenge.Queries.Categories.GetById;

public record GetCategoryByIdQueryRequest(long Id) : IRequest<GetCategoryByIdQueryResponse>;

