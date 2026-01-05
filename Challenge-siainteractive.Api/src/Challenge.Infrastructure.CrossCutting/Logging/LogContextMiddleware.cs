using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Challenge.Infrastructure.CrossCutting.Logging;

public class LogContextMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<LogContextMiddleware> logger;

    public LogContextMiddleware(RequestDelegate next, ILogger<LogContextMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public Task InvokeAsync(HttpContext context)
    {
        var correlationHeaders = Activity.Current.Baggage.Distinct().ToDictionary(b => b.Key, b => (object)b.Value);

        // ensures all entries are tagged with some values
        using (logger.BeginScope(correlationHeaders))
        {
            // Call the next delegate/middleware in the pipeline
            return next(context);
        }
    }
}
