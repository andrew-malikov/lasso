using System.ComponentModel.DataAnnotations;

namespace Users.Application.Users;

public abstract class RegisterResponse;

public sealed class SuccessfullyRegistered : RegisterResponse;

public sealed class InvalidUserDraft : RegisterResponse
{
    public required ValidationException Exception { get; init; }
}

public sealed class DuplicateUser : RegisterResponse
{
    public required string Message { get; init; }
}