using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Challenge.Infrastructure.CrossCutting.HealthCheck;

public static class HealthCheckExtension
{
    public static void AddApiHealthChecks(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddCheck<WhatchdogFileHealthCheck>("Watchdog File Check", HealthStatus.Unhealthy,
                new[] { "watchdog", "file" });
    }
}
