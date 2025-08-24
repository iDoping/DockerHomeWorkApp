using DockerHomeWorkApp.Repositories;
using DockerHomeWorkApp.Security;
using DockerHomeWorkApp.Services;

namespace DockerHomeWorkApp.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProjectServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        return services;
    }
}