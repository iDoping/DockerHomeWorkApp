using DockerHomeWorkApp.Repositories;

namespace DockerHomeWorkApp.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var env = app.ServiceProvider.GetRequiredService<IHostEnvironment>();
        if (!env.IsDevelopment())
        {
            return;
        }

        var group = app.MapGroup("/users").RequireAuthorization();

        //group.MapGet("/", async (IUsersRepository usersRepository, CancellationToken ct) =>
        //{
        //    var users = await usersRepository.GetAllAsync(ct);
        //    return Results.Ok(users);
        //});

        group.MapGet("/{id:int}", async (int id, IUsersRepository usersRepository, CancellationToken ct) =>
        {
            var user = await usersRepository.GetByIdAsync(id, ct);
            return user is null ? Results.NotFound() : Results.Ok(user);
        });

        //group.MapDelete("/{id:int}", async (int id, IUsersRepository usersRepository, CancellationToken ct) =>
        //{
        //    var deleted = await usersRepository.DeleteAsync(id, ct);
        //    return deleted ? Results.NoContent() : Results.NotFound();
        //});
    }
}

