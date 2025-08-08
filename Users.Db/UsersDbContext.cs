using Microsoft.EntityFrameworkCore;

namespace Users.Db;

public class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(b =>
        {
            b.ToTable("users");
            b.HasKey(p => p.Id);
            b.Property(p => p.Username).HasMaxLength(60);
            b.HasIndex(u => u.Username).IsUnique().HasDatabaseName("users_username_idx");
            b.Property(p => p.Password).HasMaxLength(40);
        });
    }
}