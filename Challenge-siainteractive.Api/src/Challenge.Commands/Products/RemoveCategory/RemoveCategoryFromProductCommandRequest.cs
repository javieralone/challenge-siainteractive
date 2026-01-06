using MediatR;

namespace Challenge.Commands.Products.RemoveCategory;

public record RemoveCategoryFromProductCommandRequest(long ProductId, long CategoryId) : IRequest<RemoveCategoryFromProductCommandResponse>;

