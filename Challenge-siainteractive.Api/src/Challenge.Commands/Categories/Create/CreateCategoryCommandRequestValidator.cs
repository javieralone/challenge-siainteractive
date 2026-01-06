using FluentValidation;

namespace Challenge.Commands.Categories.Create;

public class CreateCategoryCommandRequestValidator : AbstractValidator<CreateCategoryCommandRequest>
{
    public CreateCategoryCommandRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
