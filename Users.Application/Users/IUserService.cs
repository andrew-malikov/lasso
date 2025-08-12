namespace Users.Application.Users;

public interface IUserService
{
    Task<RegisterResponse> Register(UserDraft draft, CancellationToken token = default);

    Task<LoginResponse> Login(LoginRequest request, CancellationToken token = default);

    Task Logout(string refreshToken, CancellationToken token = default);
}