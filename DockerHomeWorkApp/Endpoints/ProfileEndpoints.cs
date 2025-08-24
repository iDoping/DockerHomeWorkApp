using DockerHomeWorkApp.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DockerHomeWorkApp.Endpoints;

public static class ProfileEndpoints
{
    public record UpdateProfileRequest(string? FirstName, string? LastName);

    public static IEndpointRouteBuilder MapProfileEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/").RequireAuthorization();

        g.MapGet("me", async (ClaimsPrincipal user, IUsersRepository repo, CancellationToken ct) =>
        {
            var id = long.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
            var u = await repo.GetByIdAsync(id, ct);
            return u is null
                ? Results.NotFound()
                : Results.Ok(new { id = u.Id, email = u.Email, firstName = u.FirstName, lastName = u.LastName });
        });

        g.MapPut("me", async (ClaimsPrincipal user, UpdateProfileRequest body, IUsersRepository repo, CancellationToken ct) =>
        {
            var id = long.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
            await repo.UpdateNamesAsync(id, body.FirstName, body.LastName, ct);
            return Results.Ok();
        });

        return app;
    }
}