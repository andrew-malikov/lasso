using System.Security.Cryptography;
using AwesomeAssertions;
using Users.Application.Jwt;
using Users.Application.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Options;

namespace Users.Application.UnitTests;

public class JwtTokenFactoryTests
{
    private readonly JwtTokenFactory _sut;
    private readonly AuthenticationSettings _settings;

    public JwtTokenFactoryTests()
    {
        using var rsa = RSA.Create(2048);
        var certReq = new CertificateRequest("CN=Test", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var cert = certReq.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(365));

        _settings = new AuthenticationSettings
        {
            Issuer = "TestIssuer",
            Audience = ["aud1", "aud2"],
            AccessTokenExpiration = TimeSpan.FromMinutes(5),
            RefreshTokenExpiration = TimeSpan.FromDays(7),
            SigningCertificate = cert
        };

        var options = Options.Create(_settings);
        _sut = new JwtTokenFactory(options);
    }

    private JwtSecurityToken ReadToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        return handler.ReadJwtToken(token);
    }

    [Fact]
    public void CreateAccessToken_ShouldContainExpectedClaims()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), "john", "hashed");

        // Act
        var tokenString = _sut.CreateAccessToken(user);
        var token = ReadToken(tokenString);

        // Assert
        token.Issuer.Should().Be(_settings.Issuer);
        token.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
        token.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Aud && c.Value == "aud1");
        token.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Aud && c.Value == "aud2");

        token.ValidTo.Should().BeCloseTo(DateTime.UtcNow.Add(_settings.AccessTokenExpiration), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void CreateRefreshToken_ShouldContainExpectedClaims_AndReturnExpiresAt()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), "john", "hashed");

        // Act
        var (tokenString, expiresAt) = _sut.CreateRefreshToken(user);
        var token = ReadToken(tokenString);

        // Assert
        token.Issuer.Should().Be(_settings.Issuer);
        token.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
        token.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Aud && c.Value == "aud1");
        token.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Aud && c.Value == "aud2");

        token.ValidTo.Should().BeCloseTo(expiresAt.UtcDateTime, TimeSpan.FromSeconds(5));
        expiresAt.Should().BeCloseTo(DateTimeOffset.UtcNow.Add(_settings.RefreshTokenExpiration),
            TimeSpan.FromSeconds(5));
    }
}