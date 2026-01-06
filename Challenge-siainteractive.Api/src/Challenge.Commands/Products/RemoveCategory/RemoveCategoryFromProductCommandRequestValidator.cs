using FluentValidation;

namespace Challenge.Commands.Products.RemoveCategory;

public class RemoveCategoryFromProductCommandRequestValidator : AbstractValidator<RemoveCategoryFromProductCommandRequest>
{
    public RemoveCategoryFromProductCommandRequestValidator()
    {
        RuleFor(request => request.ProductId)
            .GreaterThan(0)
            .WithMessage("Invalid ProductId");

        RuleFor(request => request.CategoryId)
            .GreaterThan(0)
            .WithMessage("Invalid CategoryId");
    }
}

