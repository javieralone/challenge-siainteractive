using FluentValidation;

namespace Challenge.Commands.Categories.Update;

public class UpdateCategoryCommandRequestValidator : AbstractValidator<UpdateCategoryCommandRequest>
{
    public UpdateCategoryCommandRequestValidator()
    {
        RuleFor(request => request.Id)
            .GreaterThan(0)
            .WithMessage("Invalid Id");

        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Requeried Name")
            .MaximumLength(100);
    }
}

