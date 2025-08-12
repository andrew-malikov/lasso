using Users.Application.Users;

namespace Users.Application.Jwt;

public interface ITokenFactory
{
    string CreateAccessToken(User user);

    string CreateRefreshToken(User user);
}