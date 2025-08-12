using Microsoft.Extensions.Caching.Distributed;

namespace Users.Application.Jwt;

internal sealed class DistributedTokenCache(IDistributedCache cache) : ITokenCache
{
    public Task StoreRefreshToken(string refreshToken, DateTimeOffset expiresAt, CancellationToken token = default)
    {
        var options = new DistributedCacheEntryOptions { AbsoluteExpiration = expiresAt };
        return cache.SetAsync(refreshToken, "1"u8.ToArray(), options, token);
    }

    public Task InvalidateRefreshToken(string refreshToken, CancellationToken token = default)
    {
        return cache.RemoveAsync(refreshToken, token);
    }
}