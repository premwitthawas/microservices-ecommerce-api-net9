using System.Net;
using Polly.CircuitBreaker;

namespace Ecommerce.OrderService.API.Middlewares;

public class ErrorHandleMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandleMiddleware> _logger;
    public ErrorHandleMiddleware(RequestDelegate next, ILogger<ErrorHandleMiddleware> logger)
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
        catch (BrokenCircuitException ex)
        {
            _logger.LogError("Circuit breaker is open: {Message}", ex.Message);

            context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable; // HTTP 503
            await context.Response.WriteAsJsonAsync(new
            {
                Message = "Service temporarily unavailable. Please try again later.",
                ExceptionType = ex.GetType().ToString()
            });
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                _logger.LogError("{ExceptionType} {ExceptionMessage}", ex.InnerException.GetType().ToString(), ex.InnerException.Message);
            }
            else
            {
                _logger.LogError("{ExceptionType} {ExceptionMessage}", ex.GetType().ToString(), ex.Message);
            }

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // HTTP 500
            await context.Response.WriteAsJsonAsync(new
            {
                Message = ex.Message,
                ExceptionType = ex.GetType().ToString()
            });
        }
    }
};

public static class ErrorHandleMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandleMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandleMiddleware>();
        return app;
    }
}