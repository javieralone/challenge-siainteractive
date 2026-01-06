using MediatR;

namespace Challenge.Commands.Products.AssignCategory;

public record AssignCategoryToProductCommandRequest(long ProductId, long CategoryId) : IRequest<AssignCategoryToProductCommandResponse>;

