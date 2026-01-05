using MediatR;

namespace Challenge.Commands.Products.Update;

public record UpdateProductCommandRequest(long Id, string Name, string Description, string? Image) : IRequest<UpdateProductCommandResponse>;

