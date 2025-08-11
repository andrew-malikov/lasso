namespace Users.WebApi.Application.User;

public interface IUserRepository
{
    Task Add(User user, CancellationToken token = default);

    Task<User?> Get(string username, CancellationToken token = default);

    Task<bool> Any(string username, CancellationToken token = default);
}