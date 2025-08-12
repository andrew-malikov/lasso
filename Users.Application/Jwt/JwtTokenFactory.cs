using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Users.Application.Users;

namespace Users.Application.Jwt;

public class JwtTokenFactory(AuthenticationSettings authenticationSettings) : ITokenFactory
{
    private static readonly JwtSecurityTokenHandler TokenHandler = new();

    public string CreateAccessToken(User user)
    {
        var claims = new List<Claim>(2 + authenticationSettings.Audience.Length)
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var audience in authenticationSettings.Audience)
        {
            claims.Add(new(JwtRegisteredClaimNames.Aud, audience));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(authenticationSettings.AccessTokenExpiration),
            Issuer = authenticationSettings.Issuer,
            SigningCredentials = new X509SigningCredentials(authenticationSettings.SigningCertificate)
        };

        var token = TokenHandler.CreateToken(tokenDescriptor);
        return TokenHandler.WriteToken(token);
    }

    public string CreateRefreshToken(User user)
    {
        var claims = new List<Claim>(2 + authenticationSettings.Audience.Length)
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var audience in authenticationSettings.Audience)
        {
            claims.Add(new(JwtRegisteredClaimNames.Aud, audience));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(authenticationSettings.RefreshTokenExpiration),
            Issuer = authenticationSettings.Issuer,
            SigningCredentials = new X509SigningCredentials(authenticationSettings.SigningCertificate)
        };

        var token = TokenHandler.CreateToken(tokenDescriptor);
        return TokenHandler.WriteToken(token);
    }
}