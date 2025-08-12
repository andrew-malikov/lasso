using Microsoft.EntityFrameworkCore;
using Npgsql;
using Users.Application.Users;

namespace Users.Db;

public class UserRepository(UsersDbContext dbContext) : IUserRepository
{
    public async Task Add(User user, CancellationToken token = default)
    {
        dbContext.Users.Add(user.MapTo());
        try
        {
            await dbContext.SaveChangesAsync(token);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new DuplicateUserException(user.Username);
        }
    }

    public Task<User?> Get(string username, CancellationToken token = default)
    {
        return dbContext.Users
            .AsNoTracking()
            .Where(u => u.Username == username)
            .Select(u => new User(u.Id, u.Username, u.Password))
            .FirstOrDefaultAsync(token);
    }

    public Task<bool> Any(string username, CancellationToken token = default)
    {
        return dbContext.Users.AnyAsync(u => u.Username == username, token);
    }


    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        if (ex.InnerException is PostgresException pgEx)
        {
            return pgEx.SqlState == PostgresErrorCodes.UniqueViolation;
        }

        return false;
    }
}

internal static class UserExtensions
{
    public static UserEntity MapTo(this User user)
    {
        return new UserEntity
        {
            Id = user.Id,
            Password = user.Password,
            Username = user.Username
        };
    }
}