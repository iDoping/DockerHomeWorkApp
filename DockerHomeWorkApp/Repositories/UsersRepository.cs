using DockerHomeWorkApp.DataAccess.AppDataModel;
using DockerHomeWorkApp.DataContext;
using Microsoft.EntityFrameworkCore;

namespace DockerHomeWorkApp.Repositories;

public interface IUsersRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<User?> GetByIdAsync(long id, CancellationToken ct);
    Task<long> CreateAsync(string email, byte[] hash, byte[] salt, string? first, string? last, CancellationToken ct);
    Task UpdateNamesAsync(long id, string? first, string? last, CancellationToken ct);
}

public class UsersRepository(AppDataContext ctx) : IUsersRepository
{
    private readonly AppDataContext _ctx = ctx;

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct)
    {
        return await _ctx.Users.AsNoTracking().ToListAsync(ct);
    }

    public Task<User?> GetByIdAsync(long id, CancellationToken ct)
    {
        return _ctx.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<long> CreateAsync(string email, byte[] hash, byte[] salt, string? first, string? last, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(hash);
        ArgumentNullException.ThrowIfNull(salt);

        var now = DateTimeOffset.UtcNow;

        var user = new User
        {
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            FirstName = first,
            LastName = last,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _ctx.Users.AddAsync(user, ct);
        await _ctx.SaveChangesAsync(ct);

        return user.Id;
    }

    public async Task UpdateNamesAsync(long id, string? first, string? last, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var affected = await _ctx.Users
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(u => u.FirstName, first)
                .SetProperty(u => u.LastName, last)
                .SetProperty(u => u.UpdatedAt, now),
                ct);
        if (affected == 0)
            throw new KeyNotFoundException($"User {id} not found.");
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct)
    {
        var user = await _ctx.Users.FindAsync([id], ct);
        if (user == null) return false;

        _ctx.Users.Remove(user);
        await _ctx.SaveChangesAsync(ct);
        return true;
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct) =>
        _ctx.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct) =>
        _ctx.Users.AsNoTracking().AnyAsync(u => u.Email == email, ct);

    public Task AddAsync(User user, CancellationToken ct) =>
        _ctx.Users.AddAsync(user, ct).AsTask();

    public Task SaveChangesAsync(CancellationToken ct) =>
        _ctx.SaveChangesAsync(ct);
}