using FluentValidation;
using MediatR;
using Newtonsoft.Json.Converters;
using Serilog.Events;
using Challenge.Api.Behaviors;
using Challenge.Commands;
using Challenge.Domain.Services;
using Challenge.Infrastructure.CrossCutting.Authentications;
using Challenge.Infrastructure.CrossCutting.HealthCheck;
using Challenge.Infrastructure.CrossCutting.Storage;
using Challenge.Infrastructure.CrossCutting.Swagger;
using Challenge.Infrastructure.Data;
using Challenge.Queries;

namespace Challenge.Api.ConfigureServices;

internal static class StartupExtensions
{
    internal static void AddConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApiVersioning();
        services.AddSwagger();
        services.AddBearerAuthentication(configuration);
        services.AddSwaggerGenNewtonsoftSupport();
        services.AddApiHealthChecks();
        services.AddMediatrHandlers();
        services.AddPipelineBehaviors();
        services.AddRepositories();
        services.AddScoped<IImageStorageService, ImageStorageService>();
        services.AddControllers().AddNewtonsoftJson(options =>
            options.SerializerSettings.Converters.Add(new StringEnumConverter()));
    }

    internal static void AddMediatrHandlers(this IServiceCollection services)
    {
        services.AddMediatR(typeof(MediatrQueriesEntry).Assembly, typeof(MediatrQueriesEntry).Assembly);
        services.AddMediatR(typeof(MediatrCommandsEntry).Assembly, typeof(MediatrCommandsEntry).Assembly);
    }

    internal static void AddPipelineBehaviors(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorPipelineBehavior<,>));
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(MediatrCommandsEntry))
            .AddClasses(@class => @class.AssignableTo(typeof(IValidator<>)))
            .AsImplementedInterfaces());
    }

    public static void AddApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
            options.ReportApiVersions = true;
        });

        services.AddVersionedApiExplorer(
            options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });
    }

    internal static LogEventLevel CustomGetLevel(HttpContext ctx, double _, Exception ex) =>
           ex != null
                ? LogEventLevel.Error
                : ctx.Response.StatusCode > 499
                    ? LogEventLevel.Error
                    : IsHealthCheckEndpoint(ctx) // Not an error, check if it was a health check
                        ? LogEventLevel.Debug   // Was a health check, use Verbose
                        : LogEventLevel.Information;

    private static bool IsHealthCheckEndpoint(HttpContext ctx)
    {
        var path = ctx?.Request?.Path.Value;
        if (path != null)
        {
            if (path.ToLower().Contains("watchdog"))
                return true;
            if (path.ToLower().Contains("/health"))
                return true;
        }
        // No endpoint, so not a health check endpoint
        return false;
    }
}
