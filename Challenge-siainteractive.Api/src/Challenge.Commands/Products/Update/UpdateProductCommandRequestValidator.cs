using FluentValidation;

namespace Challenge.Commands.Products.Update;

public class UpdateProductCommandRequestValidator : AbstractValidator<UpdateProductCommandRequest>
{
    public UpdateProductCommandRequestValidator()
    {
        RuleFor(request => request.Id)
            .GreaterThan(0)
            .WithMessage("Invalid Id");

        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Required Name")
            .MaximumLength(200);

        RuleFor(request => request.Description)
            .NotEmpty()
            .WithMessage("Required Description")
            .MaximumLength(1000);

        RuleFor(request => request.Image)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Image));
    }
}

