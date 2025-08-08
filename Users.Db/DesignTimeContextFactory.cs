using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Users.Db;

public class UsersDbContextDesignTimeFactory : IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<UsersDbContext>();

        var connectionString = Environment.GetEnvironmentVariable("UsersDbContext");
        if (connectionString is null)
        {
            throw new InvalidOperationException(
                "Failed to retrieve the connection string. Environment variable 'UsersDbContext' is not set.");
        }

        builder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();

        return new UsersDbContext(builder.Options);
    }
}