using Challenge.Api.ConfigureServices;
using Challenge.Infrastructure.Data.Persistence;
using HealthChecks.UI.Client;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Challenge.Api;

public class Startup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
    }

    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Environment { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container.

        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddConfigureServices(Configuration);
        services.AddProblemDetails(pbo => ProblemDetailsConfiguration.ConfigureProblemDetails(pbo, Environment));

        services.AddDbContext<ChallengeDBContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
    }


    public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        // Configure the HTTP request pipeline.
        if (Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    // build a swagger endpoint for each discovered API version
                    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                });

        }

        app.UseSerilogRequestLogging(opts => opts.GetLevel = StartupExtensions.CustomGetLevel);
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseProblemDetails();

        app.UseStaticFiles();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
    }
}
