using System.Security.Claims;

namespace FlightServiceAPI.CustomMiddleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await LogRequest(context);
            await _next(context);
        }
        finally
        {
            await LogResponse(context);
        }
    }

    private Task LogRequest(HttpContext context)
    {
        var username = context.User?.FindFirst(ClaimTypes.Name)?.Value 
                       ?? context.User?.FindFirst("sub")?.Value 
                       ?? "Anonymous";

        _logger.LogInformation(
            "HTTP {Method} {Path} received from {IP} by User={Username} at {Now}",
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress,
            username,
            DateTime.Now);

        return Task.CompletedTask;
    }

    private Task LogResponse(HttpContext context)
    {
        var username = context.User?.FindFirst(ClaimTypes.Name)?.Value 
                       ?? context.User?.FindFirst("sub")?.Value 
                       ?? "Anonymous";

        _logger.LogInformation(
            "HTTP {Method} {Path} completed with {StatusCode} for User={Username} at {Now}",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            username,
            DateTime.Now);

        return Task.CompletedTask;
    }
}