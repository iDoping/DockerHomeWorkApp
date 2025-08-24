using DockerHomeWorkApp.Core;
using DockerHomeWorkApp.DataAccess.AppDataModel;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DockerHomeWorkApp.Security;

public interface IJwtTokenService
{
    (string token, TimeSpan expires) IssueToken(User user);
}

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _opt;
    private readonly byte[] _key;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _opt = options.Value;
        _key = Encoding.UTF8.GetBytes(_opt.Secret);
    }

    public (string token, TimeSpan expires) IssueToken(User user)
    {
        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

        var creds = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256);
        var expires = TimeSpan.FromMinutes(_opt.AccessTokenMinutes);
        var jwt = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(expires),
            signingCredentials: creds
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return (token, expires);
    }
}