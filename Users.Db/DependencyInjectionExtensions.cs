using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Application.Users;

namespace Users.Db;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddUserPersistence(this IServiceCollection self, IConfiguration configuration)
    {
        self.AddDbContextPool<UsersDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("UsersDbContext"), x => x.MigrationsAssembly("Users.Db"))
                .UseSnakeCaseNamingConvention());
        self.AddTransient<IUserRepository, UserRepository>();
        return self;
    }
}