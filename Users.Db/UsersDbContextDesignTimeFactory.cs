using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Users.Db;

public class UsersDbContextDesignTimeFactory : IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<UsersDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("UsersDbContext");
        builder.UseNpgsql(connectionString ?? "don't need one").UseSnakeCaseNamingConvention();
        return new UsersDbContext(builder.Options);
    }
}