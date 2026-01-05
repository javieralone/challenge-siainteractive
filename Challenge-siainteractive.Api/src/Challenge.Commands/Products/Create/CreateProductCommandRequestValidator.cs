using FluentValidation;

namespace Challenge.Commands.Products.Create;

public class CreateProductCommandRequestValidator : AbstractValidator<CreateProductCommandRequest>
{
    public CreateProductCommandRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(request => request.Image)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Image));
    }
}

