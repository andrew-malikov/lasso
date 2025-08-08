namespace Users.GrpcApi.Domain;

public class UserTokens
{
    public required Guid UserId { get; init; }

    public required string AccessToken { get; init; }

    public required string RefreshToken { get; init; }
}