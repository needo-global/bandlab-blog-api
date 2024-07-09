using System.Net;
using System.Text.Json;
using Posts.Api.Models;

namespace Posts.Api.Middleware.Error;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            await HandleExceptionAsync(httpContext);
        }
    }

    private Task HandleExceptionAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        return context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorDto("E000", "Error Occurred")));
    }
}