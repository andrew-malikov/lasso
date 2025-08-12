namespace Users.Application.Users;

public sealed class UserTokens
{
    public required Guid UserId { get; init; }

    public required string AccessToken { get; init; }

    public required string RefreshToken { get; init; }
}