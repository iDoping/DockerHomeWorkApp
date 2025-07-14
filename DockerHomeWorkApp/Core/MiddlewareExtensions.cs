namespace DockerHomeWorkApp.Core;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<Middleware.ErrorHandlingMiddleware>();
    }
}
