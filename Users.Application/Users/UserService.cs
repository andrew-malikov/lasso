using System.Security.Cryptography;
using Medo;
using Microsoft.AspNetCore.Identity;
using Users.Application.Jwt;

namespace Users.Application.Users;

public class UserService(
    IUserRepository repository,
    IPasswordHasher<User> hasher,
    IPasswordHasher<UserDraft> draftHasher,
    ITokenFactory tokenFactory) : IUserService
{
    public async Task Register(UserDraft draft, CancellationToken token = default)
    {
        draft.Validate();

        var isUsernameTaken = await repository.Any(draft.Username, token);
        if (isUsernameTaken)
        {
            throw new DuplicateUserException(draft.Username);
        }

        var salt = Salt.GenerateEncoded();
        var passwordHash = draftHasher.HashPassword(draft, draft.Password + salt);
        var user = new User(Uuid7.NewGuid(), draft.Username, passwordHash, salt);
        await repository.Add(user, token);
    }

    public async Task<UserTokens> Login(LoginRequest request, CancellationToken token = default)
    {
        var user = await repository.Get(request.Username, token);
        if (user is null)
        {
            throw new NotImplementedException();
        }

        var result = hasher.VerifyHashedPassword(user, request.Password + user.Salt, user.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new NotImplementedException();
        }

        return new UserTokens
        {
            UserId = user.Id, AccessToken = tokenFactory.CreateAccessToken(user),
            RefreshToken = tokenFactory.CreateRefreshToken(user)
        };
    }

    public Task Logout(string refreshToken, CancellationToken token)
    {
        // remove all refresh tokens
        return Task.CompletedTask;
    }
}

internal static class Salt
{
    public static string GenerateEncoded(int size = 32)
    {
        var salt = new byte[size];
        RandomNumberGenerator.Fill(salt);
        return Convert.ToBase64String(salt);
    }
}