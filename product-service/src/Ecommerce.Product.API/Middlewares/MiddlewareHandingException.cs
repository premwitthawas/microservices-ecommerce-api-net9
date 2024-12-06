using System.Net;
namespace Ecommerce.Product.API.Middlewares;

public class MiddlewareHandingException
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MiddlewareHandingException> _logger;
    public MiddlewareHandingException(RequestDelegate next, ILogger<MiddlewareHandingException> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                _logger.LogError($"{ex.InnerException.GetType().ToString()} - {ex.InnerException.Message}");
            }
            else
            {
                _logger.LogError($"{ex.GetType().ToString()} - {ex.Message}");
            }
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(new
            {
                Message = ex.Message,
                Type = ex.GetType().ToString()
            });
        }
    }
}

public static class MiddlewareHandingExceptionExtension
{
    public static IApplicationBuilder UseMiddlewareHandingExceptionExtension(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MiddlewareHandingException>();
    }
}