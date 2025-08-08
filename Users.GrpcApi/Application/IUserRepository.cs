using Users.GrpcApi.Domain;

namespace Users.GrpcApi.Application;

public interface IUserRepository
{
    Task Add(User user);

    Task<User?> Get(string username);

    Task<bool> Any(string username);
}