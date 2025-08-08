using Users.Db;
using Users.GrpcApi.Domain;

namespace Users.GrpcApi.Application;

public class UserRepository : IUserRepository
{
    private readonly UsersDbContext _dbContext;

    public Task Add(User user)
    {
        return Task.CompletedTask;
    }

    public Task<User> Get(string username)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Any(string username)
    {
        throw new NotImplementedException();
    }
}