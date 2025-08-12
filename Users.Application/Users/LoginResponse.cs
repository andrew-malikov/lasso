namespace Users.Application.Users;

public abstract class LoginResponse;

public sealed class SuccessfulLogin : LoginResponse
{
    public required UserTokens Tokens { get; init; }
}

public sealed class FailedLogin : LoginResponse
{
    public required string Message { get; init; }
}