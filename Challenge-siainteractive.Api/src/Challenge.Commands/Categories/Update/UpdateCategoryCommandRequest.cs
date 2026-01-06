using MediatR;

namespace Challenge.Commands.Categories.Update;

public record UpdateCategoryCommandRequest(long Id, string Name) : IRequest<UpdateCategoryCommandResponse>;

