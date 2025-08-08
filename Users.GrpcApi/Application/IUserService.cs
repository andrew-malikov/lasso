using Users.WebApi.Domain;

namespace Users.WebApi.Application;

public interface IUserService
{
    Task Register(UserDraft draft);

    Task<UserTokens> Login(LoginRequest request);

    Task Logout();
}