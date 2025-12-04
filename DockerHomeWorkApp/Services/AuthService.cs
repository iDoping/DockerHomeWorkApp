using DockerHomeWorkApp.Repositories;
using DockerHomeWorkApp.Requests.Auth;
using DockerHomeWorkApp.Security;

namespace DockerHomeWorkApp.Services;

public interface IAuthService
{
    Task<long> RegisterAsync(RegisterRequest req, CancellationToken ct);
    Task<(string token, TimeSpan expires)> LoginAsync(LoginRequest req, CancellationToken ct);
}

public sealed class AuthService(
    IUsersRepository repo, 
    IPasswordHasher hasher, 
    IJwtTokenService jwt,
    IBillingClient billingClient
    ) : IAuthService
{
    private readonly IUsersRepository _repo = repo;
    private readonly IPasswordHasher _hasher = hasher;
    private readonly IJwtTokenService _jwt = jwt;
    private readonly IBillingClient _billingClient = billingClient;

    public async Task<long> RegisterAsync(RegisterRequest req, CancellationToken ct)
    {
        var existing = await _repo.GetByEmailAsync(req.Email, ct);

        if (existing is not null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var (hash, salt) = _hasher.Hash(req.Password);

        var id = await _repo.CreateAsync(
            req.Email,
            hash,
            salt,
            req.FirstName,
            req.LastName,
            ct);

        await _billingClient.CreateAccountAsync(id, ct);

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