namespace DockerHomeWorkApp.Services;

public interface IBillingClient
{
    Task CreateAccountAsync(long userId, CancellationToken ct);
}

public sealed class BillingClient(HttpClient http) : IBillingClient
{
    private readonly HttpClient _http = http;

    private sealed record CreateAccountRequest(long UserId);

    public async Task CreateAccountAsync(long userId, CancellationToken ct)
    {
        var response = await _http.PostAsJsonAsync("/billing/accounts", new CreateAccountRequest(userId), ct);

        response.EnsureSuccessStatusCode();
    }
}