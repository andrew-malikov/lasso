using Users.GrpcApi.Domain;

namespace Users.GrpcApi.Application;

public interface ITokenFactory
{
    string CreateAccessToken(User user);

    string CreateRefreshToken(User user);
}