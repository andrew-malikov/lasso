namespace Users.Application.Users;

public abstract class LoginResponse;

public class SuccessfulLogin : LoginResponse
{
    public required UserTokens Tokens { get; init; }
}

public class FailedLogin : LoginResponse
{
    public required string Message { get; init; }
}