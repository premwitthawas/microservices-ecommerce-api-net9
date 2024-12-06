using System.Net;

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
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                _logger.LogError($"{ex.InnerException.GetType().ToString()} : {ex.InnerException.Message}");
            }
            else
            {
                _logger.LogError($"{ex.GetType().ToString()} : {ex.Message}");
                var response = new { message = ex.Message, Type = ex.GetType().ToString() };
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(response);
            }
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