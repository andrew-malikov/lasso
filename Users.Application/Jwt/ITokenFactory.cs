using Users.Application.Users;

namespace Users.Application.Jwt;

public interface ITokenFactory
{
    string CreateAccessToken(User user);

    (string token, DateTimeOffset expiresAt) CreateRefreshToken(User user);
}