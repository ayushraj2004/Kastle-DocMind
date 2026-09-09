using System.Text.Json;
namespace Kastle.DocMind.Api.Middleware;
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next =next;
        _logger=logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception,"unhandled exception occurred");
            context.Response.StatusCode=StatusCodes.Status500InternalServerError;
            context.Response.ContentType="application/json";
            var response = new
            {
                message="unexpected error occurred"
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}