using MediatR;

namespace Challenge.Commands.Products.Create;

public record CreateProductCommandRequest(string Name, string Description, string? Image) : IRequest<CreateProductCommandResponse>;

