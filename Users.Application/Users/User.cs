namespace Users.Application.Users;

public sealed class User(Guid id, string username, string password)
{
    public Guid Id { get; init; } = id;

    /// <summary>
    ///     Unique username.
    /// </summary>
    public string Username { get; init; } = username;

    /// <summary>
    ///     A hashed pair of original password and salt. 
    /// </summary>
    public string Password { get; init; } = password;
}