namespace Users.Application.Users;

public class DuplicateUserException : Exception
{
    public DuplicateUserException(string username)
        : base($"A user with username '{username}' already exists.")
    {
    }
}