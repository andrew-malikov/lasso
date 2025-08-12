using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Users.Application.Users;

namespace Users.Application.Jwt;

public class JwtTokenFactory(IOptions<AuthenticationSettings> authenticationSettings) : ITokenFactory
{
    private static readonly JwtSecurityTokenHandler TokenHandler = new();

    public string CreateAccessToken(User user)
    {
        var claims = new List<Claim>(2 + authenticationSettings.Value.Audience.Length)
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var audience in authenticationSettings.Value.Audience)
        {
            claims.Add(new(JwtRegisteredClaimNames.Aud, audience));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(authenticationSettings.Value.AccessTokenExpiration),
            Issuer = authenticationSettings.Value.Issuer,
            SigningCredentials = new X509SigningCredentials(authenticationSettings.Value.SigningCertificate)
        };

        var token = TokenHandler.CreateToken(tokenDescriptor);
        return TokenHandler.WriteToken(token);
    }

    public (string token, DateTimeOffset expiresAt) CreateRefreshToken(User user)
    {
        var claims = new List<Claim>(2 + authenticationSettings.Value.Audience.Length)
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var audience in authenticationSettings.Value.Audience)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, audience));
        }

        var expiresAt = DateTime.UtcNow.Add(authenticationSettings.Value.RefreshTokenExpiration);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            Issuer = authenticationSettings.Value.Issuer,
            SigningCredentials = new X509SigningCredentials(authenticationSettings.Value.SigningCertificate)
        };

        var token = TokenHandler.CreateToken(tokenDescriptor);
        return (TokenHandler.WriteToken(token), expiresAt);
    }
}