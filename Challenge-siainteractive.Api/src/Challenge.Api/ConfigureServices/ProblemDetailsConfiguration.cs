using Challenge.Domain.Exceptions;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;

namespace Challenge.Api.ConfigureServices;

internal static class ProblemDetailsConfiguration
{
    internal static void ConfigureProblemDetails(ProblemDetailsOptions options, IWebHostEnvironment environment)
    {
        // This is the default behavior; only include exception details in a development environment.
        options.IncludeExceptionDetails = (ctx, ex) => environment.IsDevelopment();
        // options.IncludeExceptionDetails = (ctx, ex) => true;

        // This will map NotImplementedException to the 501 Not Implemented status code.
        options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);

        // This will map HttpRequestException to the 503 Service Unavailable status code.
        options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);

        options.MapToStatusCode<ValidationException>(StatusCodes.Status400BadRequest);

        options.MapToStatusCode<InvalidImageFileException>(StatusCodes.Status400BadRequest);

        options.MapToStatusCode<DuplicatedProductException>(StatusCodes.Status400BadRequest);

        options.MapToStatusCode<ProductNotFoundException>(StatusCodes.Status404NotFound);

        options.MapToStatusCode<DuplicatedCategoryException>(StatusCodes.Status404NotFound);

        options.MapToStatusCode<CategoryNotFoundException>(StatusCodes.Status404NotFound);
        
        options.MapToStatusCode<DuplicatedProductCategoryException>(StatusCodes.Status400BadRequest);

        options.MapToStatusCode<ProductCategoryNotFoundException>(StatusCodes.Status404NotFound);

        // Because exceptions are handled polymorphically, this will act as a "catch all" mapping, which is why it's added last.
        // If an exception other than NotImplementedException and HttpRequestException is thrown, this will handle it.
        options.Map<Exception>(ex => new StatusCodeProblemDetails(StatusCodes.Status500InternalServerError));
    }
}
