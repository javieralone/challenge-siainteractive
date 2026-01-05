using MediatR;
using System.Diagnostics;

namespace Challenge.Api.Behaviors;

public class LoggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger _logger;

    public LoggingPipelineBehavior(ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Handling {{@Request}}", request);
        var stopWatch = Stopwatch.StartNew();
        var response = await next();
        stopWatch.Stop();
        _logger.LogInformation($"Handled {typeof(TRequest).Name}: {{@Response}} in {stopWatch.ElapsedMilliseconds}ms", response);
        return response;
    }
}
