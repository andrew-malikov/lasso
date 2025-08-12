using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Users.Application.Jwt;

public class AuthenticationSettingsBinder(IConfiguration configuration) : IConfigureOptions<AuthenticationSettings>
{
    public void Configure(AuthenticationSettings settings)
    {
        settings.Audience = configuration.GetRequiredValue<string[]>("Audience");

        var accessTokenExpiration = configuration.GetValue<TimeSpan?>("AccessTokenExpiration");
        if (accessTokenExpiration.HasValue)
            settings.AccessTokenExpiration = settings.AccessTokenExpiration;

        var refreshTokenExpiration = configuration.GetValue<TimeSpan?>("RefreshTokenExpiration");
        if (refreshTokenExpiration.HasValue)
            settings.RefreshTokenExpiration = settings.RefreshTokenExpiration;

        settings.Issuer = configuration.GetRequiredValue<string>("Issuer");

        settings.VerifyingCertificate =
            new X509Certificate2(configuration.GetRequiredValue<string>("VerifyingCertificate"));

        settings.SigningCertificate =
            new X509Certificate2(configuration.GetRequiredValue<string>("SigningCertificate:Path"),
                configuration.GetRequiredValue<string>("SigningCertificate:Password"));
    }
}

internal static class ConfigurationExtensions
{
    public static T GetRequiredValue<T>(this IConfiguration config, string key)
    {
        var section = config.GetRequiredSection(key);
        var value = section.Get<T>();
        if (value == null)
        {
            throw new InvalidOperationException($"Configuration value for '{key}' is missing or invalid.");
        }

        return value;
    }
}