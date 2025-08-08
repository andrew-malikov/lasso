using System.ComponentModel.DataAnnotations;

namespace Users.WebApi.Domain;

public class UserDraft
{
    /// <summary>
    ///     Unique username.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [MinLength(3), MaxLength(60)]
    public required string Username { get; init; }

    /// <summary>
    ///     Plain password.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [MinLength(8), MaxLength(40)]
    public required string Password { get; init; }
}