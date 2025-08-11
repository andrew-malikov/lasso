namespace Users.WebApi.Application.Authentication;

public interface ITokenFactory
{
    string CreateAccessToken(User.User user);

    string CreateRefreshToken(User.User user);
}