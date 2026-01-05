using Challenge.Api;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration, builder.Environment);

startup.ConfigureServices(builder.Services);

//Logger
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    var logging = hostingContext.Configuration.GetSection("Logging:LogLevel");
    var applicationName = hostingContext.Configuration.GetValue<string>("ApplicationName");

    Enum.TryParse(logging.GetValue<string>("MinimumLevel"), out LogEventLevel minimumLevel);
    Enum.TryParse(logging.GetValue<string>("SystemMinimumLevel"), out LogEventLevel systemMinimumLevel);

    loggerConfiguration
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", applicationName)
        .Filter.ByExcluding(c => c.Properties.Any(p =>
            p.Key.Contains("requestpath", StringComparison.OrdinalIgnoreCase) &&
           (p.ToString().Contains("health") || p.ToString().Contains("swagger") || p.ToString().Contains("api-docs") ||
             p.ToString().Contains("watchdog") || p.ToString().Contains("favicon"))))
        .MinimumLevel.Is(minimumLevel)
        .MinimumLevel.Override("Microsoft", systemMinimumLevel)
        .MinimumLevel.Override("System", systemMinimumLevel)
    .WriteTo.Console();
});

var app = builder.Build();

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

startup.Configure(app, apiVersionDescriptionProvider);

app.MapControllers();

app.Run();