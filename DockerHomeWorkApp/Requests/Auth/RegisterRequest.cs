namespace DockerHomeWorkApp.Requests.Auth;

public sealed record RegisterRequest(string Email, string Password, string? FirstName, string? LastName);