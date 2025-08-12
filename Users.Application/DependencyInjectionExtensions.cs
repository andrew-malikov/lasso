using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Users.Application.Jwt;
using Users.Application.Users;

namespace Users.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddUserServices(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher<object>, PasswordHasher<object>>();
        return services.AddTransient<IUserService, UserService>();
    }

    public static IServiceCollection AddJwtServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<AuthenticationSettings>();
        services.AddTransient<IConfigureOptions<AuthenticationSettings>, AuthenticationSettingsBinder>(_ =>
            new AuthenticationSettingsBinder(configuration));
        services.AddSingleton<ITokenFactory, JwtTokenFactory>();
        services.AddSingleton<ITokenCache, DistributedTokenCache>();
        return services;
    }
}