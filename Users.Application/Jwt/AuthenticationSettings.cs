using System.Security.Cryptography.X509Certificates;

namespace Users.Application.Jwt;

public class AuthenticationSettings
{
    public X509Certificate2 VerifyingCertificate { get; set; }

    public X509Certificate2 SigningCertificate { get; set; }

    public string Issuer { get; set; }

    public string[] Audience { get; set; }

    public TimeSpan AccessTokenExpiration { get; set; } = TimeSpan.FromMinutes(5);

    public TimeSpan RefreshTokenExpiration { get; set; } = TimeSpan.FromDays(30);
}