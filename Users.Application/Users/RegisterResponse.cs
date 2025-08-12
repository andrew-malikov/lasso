using System.ComponentModel.DataAnnotations;

namespace Users.Application.Users;

public abstract class RegisterResponse;

public class SuccessfullyRegistered : RegisterResponse;

public class InvalidUserDraft : RegisterResponse
{
    public required ValidationException Exception { get; init; }
}

public class DuplicateUser : RegisterResponse
{
    public required string Message { get; init; }
}