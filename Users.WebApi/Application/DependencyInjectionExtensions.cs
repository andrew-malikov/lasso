using Users.WebApi.Application.Authentication;
using Users.WebApi.Application.User;

namespace Users.WebApi.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddUserServices(this IServiceCollection self)
    {
        self.AddSingleton<ITokenFactory, JwtTokenFactory>();
        self.AddTransient<IUserRepository, UserRepository>();
        self.AddTransient<IUserService, UserService>();
        return self;
    }
}