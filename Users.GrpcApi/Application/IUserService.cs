using Users.GrpcApi.Domain;

namespace Users.GrpcApi.Application;

public interface IUserService
{
    Task Register(UserDraft draft);

    Task<UserTokens> Login(LoginRequest request);

    Task Logout();
}