using DockerHomeWorkApp.Requests.Auth;
using DockerHomeWorkApp.Services;

namespace DockerHomeWorkApp.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/auth");

        g.MapPost("/register", async (IAuthService auth, RegisterRequest req, CancellationToken ct) =>
        {
            var id = await auth.RegisterAsync(req, ct);
            return Results.Ok(new { userId = id });
        });

        g.MapPost("/login", async (IAuthService auth, LoginRequest req, CancellationToken ct) =>
        {
            try
            {
                var (token, expires) = await auth.LoginAsync(req, ct);
                return Results.Ok(new { accessToken = token, expiresIn = (int)expires.TotalSeconds });
            }
            catch (UnauthorizedAccessException) { return Results.Unauthorized(); }
        });

        return app;
    }
}
