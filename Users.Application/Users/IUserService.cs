namespace Users.Application.Users;

public interface IUserService
{
    Task Register(UserDraft draft, CancellationToken token = default);

    Task<UserTokens> Login(LoginRequest request, CancellationToken token = default);

    Task Logout(string refreshToken, CancellationToken token = default);
}