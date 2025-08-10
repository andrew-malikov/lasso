using Users.WebApi.Domain;

namespace Users.WebApi.Application;

public interface IUserRepository
{
    Task Add(User user);

    Task<User?> Get(string username);

    Task<bool> Any(string username);
}