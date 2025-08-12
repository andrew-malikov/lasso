using Medo;
using Microsoft.AspNetCore.Identity;
using Users.Application.Jwt;

namespace Users.Application.Users;

public class UserService(
    IUserRepository repository,
    IPasswordHasher<object> hasher,
    ITokenFactory tokenFactory,
    ITokenCache tokenCache) : IUserService
{
    public async Task<RegisterResponse> Register(UserDraft draft, CancellationToken token = default)
    {
        var validationException = draft.Validate();
        if (validationException is not null)
        {
            return new InvalidUserDraft { Exception = validationException };
        }

        var isUsernameTaken = await repository.Any(draft.Username, token);
        if (isUsernameTaken)
        {
            return new DuplicateUser { Message = $"Username '{draft.Username}' is already taken." };
        }

        var passwordHash = hasher.HashPassword(draft, draft.Password);
        var user = new User(Uuid7.NewGuid(), draft.Username, passwordHash);
        try
        {
            await repository.Add(user, token);
        }
        catch (DuplicateUserException)
        {
            return new DuplicateUser { Message = $"Username '{draft.Username}' is already taken." };
        }

        return new SuccessfullyRegistered();
    }

    public async Task<LoginResponse> Login(LoginRequest request, CancellationToken token = default)
    {
        var user = await repository.Get(request.Username, token);
        if (user is null)
        {
            return new FailedLogin { Message = $"User's not found by username '{request.Username}'." };
        }

        var result = hasher.VerifyHashedPassword(user, user.Password, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return new FailedLogin { Message = "User's password doesn't match." };
        }

        var (refreshToken, expiresAt) = tokenFactory.CreateRefreshToken(user);

        await tokenCache.StoreRefreshToken(refreshToken, expiresAt, token);

        return new SuccessfulLogin
        {
            Tokens = new UserTokens
            {
                UserId = user.Id, AccessToken = tokenFactory.CreateAccessToken(user),
                RefreshToken = refreshToken
            }
        };
    }

    public Task Logout(string refreshToken, CancellationToken token)
    {
        return tokenCache.InvalidateRefreshToken(refreshToken, token);
    }
}