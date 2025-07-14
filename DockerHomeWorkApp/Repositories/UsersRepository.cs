using DockerHomeWorkApp.DataAccess.AppDataModel;
using DockerHomeWorkApp.DataContext;
using DockerHomeWorkApp.Requests;
using Microsoft.EntityFrameworkCore;

namespace DockerHomeWorkApp.Repositories;

public interface IUsersRepository
{
    Task<IEnumerable<User>> GetAllAsync(CancellationToken ct);
    Task<User?> GetByIdAsync(int id, CancellationToken ct);
    Task<User> CreateAsync(CreateUserRequest request, CancellationToken ct);
    Task<User?> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}

public class UsersRepository(AppDataContext ctx) : IUsersRepository
{
    private readonly AppDataContext _ctx = ctx;

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct)
    {
        return await _ctx.Users.AsNoTracking().ToListAsync(ct);
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _ctx.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<User> CreateAsync(CreateUserRequest request, CancellationToken ct)
    {
        var user = new User
        {
            Username = request.Username,
            Email = request.Email
        };

        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync(ct);
        return user;
    }

    public async Task<User?> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct)
    {
        var user = await _ctx.Users.FindAsync(id, ct);

        if (user == null) return null;

        user.Username = request.Username ?? string.Empty;
        user.Email = request.Email ?? string.Empty;

        await _ctx.SaveChangesAsync(ct);
        return user;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var user = await _ctx.Users.FindAsync(id, ct);

        if (user == null) return false;

        _ctx.Users.Remove(user);

        await _ctx.SaveChangesAsync(ct);
        return true;
    }
}