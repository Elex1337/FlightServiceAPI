using System.Diagnostics;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlightServiceAPI.Application.Common.Behaviours;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid();
        var requestName = request.GetType().Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Starting request {RequestName} [{CorrelationId}]. Request: {Request}",
            requestName,
            correlationId,
            JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = true }));

        var response = await next();
        stopwatch.Stop();

        _logger.LogInformation("Completed request {RequestName} [{CorrelationId}] in {ElapsedMilliseconds}ms",
            requestName,
            correlationId,
            stopwatch.ElapsedMilliseconds);

        return response;
    }
}