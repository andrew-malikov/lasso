namespace Users.GrpcApi.Domain;

public class User(Guid id, string username, string password, string salt)
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

    /// <summary>
    ///     User specific salt for improved password hash tampering.
    /// </summary>
    public string Salt { get; init; } = salt;
}