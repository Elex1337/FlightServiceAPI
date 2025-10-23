using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace FlightServiceAPI.CustomMiddleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }
    private async Task HandleException(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        
        var problemDetails = new ProblemDetails
        {
            Status = exception switch
            {
                ValidationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            },
            Title = exception switch
            {
                ValidationException => "Validation Error",
                _ => "Internal Server Error"
            },
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        _logger.LogError(exception, $"{problemDetails.Title}: {exception.Message}");

        context.Response.StatusCode = problemDetails.Status.Value;
        
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}