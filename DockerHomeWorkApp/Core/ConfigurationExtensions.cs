using Microsoft.Extensions.Options;

namespace DockerHomeWorkApp.Core;

public static class ConfigurationExtensions
{
    public static OptionsBuilder<T> BindValidated<T>(this IServiceCollection services, IConfiguration cfg, string sectionName)
        where T : class
    {
        var section = cfg.GetRequiredSection(sectionName);
        return services.AddOptions<T>()
                       .Bind(section)
                       .ValidateDataAnnotations()
                       .ValidateOnStart();
    }
}