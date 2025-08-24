namespace DockerHomeWorkApp.Core;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; set; } = "DockerHomeWorkApp";
    public string Audience { get; set; } = "DockerHomeWorkApp";
    public string Secret { get; set; } = default!;
    public int AccessTokenMinutes { get; set; } = 60;
}
