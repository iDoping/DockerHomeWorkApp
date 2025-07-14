using System.Net;
using System.Text.Json;

namespace DockerHomeWorkApp.Middleware;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new { error = "Internal Server Error", detail = ex.Message };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
