using MediatR;

namespace Challenge.Commands.Categories.Create;

public record CreateCategoryCommandRequest(string Name) : IRequest<CreateCategoryCommandResponse>;
