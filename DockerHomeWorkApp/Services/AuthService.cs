using DockerHomeWorkApp.Repositories;
using DockerHomeWorkApp.Requests.Auth;
using DockerHomeWorkApp.Security;

namespace DockerHomeWorkApp.Services;

public interface IAuthService
{
    Task<long> RegisterAsync(RegisterRequest req, CancellationToken ct);
    Task<(string token, TimeSpan expires)> LoginAsync(LoginRequest req, CancellationToken ct);
}

public sealed class AuthService(IUsersRepository repo, IPasswordHasher hasher, IJwtTokenService jwt) : IAuthService
{
    private readonly IUsersRepository _repo = repo;
    private readonly IPasswordHasher _hasher = hasher;
    private readonly IJwtTokenService _jwt = jwt;

    public async Task<long> RegisterAsync(RegisterRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Email)) throw new ArgumentException("Email is required");
        if (string.IsNullOrWhiteSpace(req.Password)) throw new ArgumentException("Password is required");

        var existing = await _repo.GetByEmailAsync(req.Email, ct);
        if (existing is not null) throw new InvalidOperationException("Email already exists");

        var (hash, salt) = _hasher.Hash(req.Password);
        var id = await _repo.CreateAsync(req.Email, hash, salt, req.FirstName, req.LastName, ct);
        return id;
    }

    public async Task<(string token, TimeSpan expires)> LoginAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _repo.GetByEmailAsync(req.Email, ct);
        if (user is null) throw new UnauthorizedAccessException();

        if (!_hasher.Verify(req.Password, user.PasswordHash, user.PasswordSalt))
            throw new UnauthorizedAccessException();

        var (token, expires) = _jwt.IssueToken(user);
        return (token, expires);
    }
}