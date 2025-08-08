using System.Data.Common;
using Medo;
using Microsoft.AspNetCore.Identity;
using Users.GrpcApi.Domain;

namespace Users.GrpcApi.Application;

public class UserService(
    IUserRepository repository,
    IPasswordHasher<User> hasher,
    IPasswordHasher<UserDraft> draftHasher,
    ITokenFactory tokenFactory,
    ILogger<UserService> logger) : IUserService
{
    public async Task Register(UserDraft draft)
    {
        // first: validate the draft

        var isUsernameTaken = !await repository.Any(draft.Username);
        if (isUsernameTaken)
        {
            throw new NotImplementedException();
        }

        var salt = "generated_salt";
        var passwordHash = draftHasher.HashPassword(draft, draft.Password + salt);
        var user = new User(Uuid7.NewGuid(), draft.Username, passwordHash, salt);
        try
        {
            await repository.Add(user);
        }
        // todo: check the constraint/ index error
        catch (DbException)
        {
            throw new NotImplementedException();
        }
    }

    public async Task<UserTokens> Login(LoginRequest request)
    {
        var user = await repository.Get(request.Username);
        if (user is null)
        {
            throw new NotImplementedException();
        }

        var result = hasher.VerifyHashedPassword(user, request.Password + "generated_salt", user.Password);
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

    public Task Logout()
    {
        // remove all refresh tokens
        return Task.CompletedTask;
    }
}