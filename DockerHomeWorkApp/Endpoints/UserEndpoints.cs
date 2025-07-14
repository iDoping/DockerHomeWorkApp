using DockerHomeWorkApp.Repositories;
using DockerHomeWorkApp.Requests;

namespace DockerHomeWorkApp.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users");

        group.MapGet("/", async (IUsersRepository usersRepository, CancellationToken ct) =>
        {
            var users = await usersRepository.GetAllAsync(ct);
            return Results.Ok(users);
        });

        group.MapGet("/{id:int}", async (int id, IUsersRepository usersRepository, CancellationToken ct) =>
        {
            var user = await usersRepository.GetByIdAsync(id, ct);
            return user is null ? Results.NotFound() : Results.Ok(user);
        });

        group.MapPost("/", async (CreateUserRequest request, IUsersRepository usersRepository, CancellationToken ct) =>
        {
            var created = await usersRepository.CreateAsync(request, ct);
            return Results.Created($"/users/{created.Id}", created);
        });

        group.MapPut("/{id:int}", async (int id, UpdateUserRequest request, IUsersRepository usersRepository, CancellationToken ct) =>
        {
            var updated = await usersRepository.UpdateAsync(id, request, ct);
            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        group.MapDelete("/{id:int}", async (int id, IUsersRepository usersRepository, CancellationToken ct) =>
        {
            var deleted = await usersRepository.DeleteAsync(id, ct);
            return deleted ? Results.NoContent() : Results.NotFound();
        });
    }
}

