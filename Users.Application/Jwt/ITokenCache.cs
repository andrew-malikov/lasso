namespace Users.Application.Jwt;

public interface ITokenCache
{
    Task StoreRefreshToken(string refreshToken, DateTimeOffset expiresAt, CancellationToken token = default);
    Task InvalidateRefreshToken(string refreshToken, CancellationToken token = default);
}