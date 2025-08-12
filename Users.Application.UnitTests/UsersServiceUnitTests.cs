using AwesomeAssertions;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Users.Application.Jwt;
using Users.Application.Users;

namespace Users.Application.UnitTests;

public class UserServiceTests
{
    private readonly IUserRepository _repository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher<object> _hasher = Substitute.For<IPasswordHasher<object>>();
    private readonly ITokenFactory _tokenFactory = Substitute.For<ITokenFactory>();
    private readonly ITokenCache _tokenCache = Substitute.For<ITokenCache>();

    private readonly UserService _sut;

    public UserServiceTests()
    {
        _sut = new UserService(_repository, _hasher, _tokenFactory, _tokenCache);
    }

    [Fact]
    public async Task Register_ShouldReturnInvalidUserDraft_WhenDraftIsInvalid()
    {
        // Arrange
        var draft = new UserDraft { Username = "", Password = "" };

        // Act
        var result = await _sut.Register(draft);

        // Assert
        result.Should().BeOfType<InvalidUserDraft>();
    }

    [Fact]
    public async Task Register_ShouldReturnDuplicateUser_WhenUsernameAlreadyExists()
    {
        // Arrange
        var draft = new UserDraft { Username = "boris", Password = "nopassword" };

        _repository.Any("boris", Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _sut.Register(draft);

        // Assert
        result.Should().BeOfType<DuplicateUser>()
            .Which.Message.Should().Contain("boris");
    }

    [Fact]
    public async Task Register_ShouldReturnSuccessfullyRegistered_WhenUserIsNew()
    {
        // Arrange
        var draft = new UserDraft { Username = "banana", Password = "almostnone" };

        _repository.Any("banana", Arg.Any<CancellationToken>()).Returns(false);
        _hasher.HashPassword(draft, "almostnone").Returns("hashed");

        // Act
        var result = await _sut.Register(draft);

        // Assert
        result.Should().BeOfType<SuccessfullyRegistered>();
        await _repository.Received(1)
            .Add(
                Arg.Is<User>(u => u.Username == "banana" && u.Password == "hashed"),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Login_ShouldReturnFailedLogin_WhenUserNotFound()
    {
        // Arrange
        var request = new LoginRequest { Username = "blake", Password = "shallpass" };
        _repository.Get("blake", Arg.Any<CancellationToken>()).Returns((User)null!);

        // Act
        var result = await _sut.Login(request);

        // Assert
        result.Should().BeOfType<FailedLogin>();
    }

    [Fact]
    public async Task Login_ShouldReturnFailedLogin_WhenPasswordDoesNotMatch()
    {
        // Arrange
        var request = new LoginRequest { Username = "john", Password = "wrong" };
        var user = new User(Guid.NewGuid(), "john", "hashed");

        _repository.Get("john", Arg.Any<CancellationToken>()).Returns(user);
        _hasher
            .VerifyHashedPassword(user, "hashed", "wrong")
            .Returns(PasswordVerificationResult.Failed);

        // Act
        var result = await _sut.Login(request);

        // Assert
        result.Should().BeOfType<FailedLogin>();
    }

    [Fact]
    public async Task Login_ShouldReturnSuccessfulLogin_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new LoginRequest { Username = "bunny", Password = "notpassed" };
        var user = new User(Guid.NewGuid(), "bunny", "hashed");

        _repository.Get("bunny", Arg.Any<CancellationToken>()).Returns(user);
        _hasher.VerifyHashedPassword(user, "hashed", "notpassed")
            .Returns(PasswordVerificationResult.Success);

        _tokenFactory.CreateRefreshToken(user).Returns(("refresh-token", DateTime.UtcNow.AddDays(1)));
        _tokenFactory.CreateAccessToken(user).Returns("access-token");

        // Act
        var result = await _sut.Login(request);

        // Assert
        result.Should().BeOfType<SuccessfulLogin>()
            .Which.Tokens.Should().BeEquivalentTo(new UserTokens
            {
                UserId = user.Id,
                AccessToken = "access-token",
                RefreshToken = "refresh-token"
            });

        await _tokenCache
            .Received(1)
            .StoreRefreshToken("refresh-token", Arg.Any<DateTimeOffset>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Logout_ShouldInvalidateRefreshToken()
    {
        // Act
        await _sut.Logout("refresh-token", CancellationToken.None);

        // Assert
        await _tokenCache.Received(1).InvalidateRefreshToken("refresh-token", Arg.Any<CancellationToken>());
    }
}