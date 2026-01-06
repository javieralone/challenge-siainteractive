using FluentValidation;

namespace Challenge.Commands.Products.AssignCategory;

public class AssignCategoryToProductCommandRequestValidator : AbstractValidator<AssignCategoryToProductCommandRequest>
{
    public AssignCategoryToProductCommandRequestValidator()
    {
        RuleFor(request => request.ProductId)
            .GreaterThan(0)
            .WithMessage("Invalid ProductId");

        RuleFor(request => request.CategoryId)
            .GreaterThan(0)
            .WithMessage("Invalid CategoryId");
    }
}

