using Users.WebApi.Domain;

namespace Users.WebApi.Application;

public interface ITokenFactory
{
    string CreateAccessToken(User user);

    string CreateRefreshToken(User user);
}