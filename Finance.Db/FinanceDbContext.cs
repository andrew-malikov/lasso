using Microsoft.EntityFrameworkCore;

namespace Finance.Db;


public class FinanceDbContext(DbContextOptions<FinanceDbContext> options) : DbContext(options)
{
    public DbSet<CurrencyEntity> Currencies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CurrencyEntity>(b =>
        {
            b.ToTable("currencies");
            b.HasKey(p => p.Id);
            b.Property(p => p.Name).HasMaxLength(60);
            b.Property(p => p.Rate).HasPrecision(20, 10);
        });
    }
}