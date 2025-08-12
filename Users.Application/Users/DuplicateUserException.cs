namespace Users.Application.Users;

public sealed class DuplicateUserException : Exception
{
    public DuplicateUserException(string username)
        : base($"A user with username '{username}' already exists.")
    {
    }
}