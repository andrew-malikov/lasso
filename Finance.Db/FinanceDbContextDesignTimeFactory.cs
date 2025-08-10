using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Finance.Db;

public class FinanceDbContextDesignTimeFactory : IDesignTimeDbContextFactory<FinanceDbContext>
{
    public FinanceDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<FinanceDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("FinanceDbContext");
        builder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        return new FinanceDbContext(builder.Options);
    }
}