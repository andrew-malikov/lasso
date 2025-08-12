using System.ComponentModel.DataAnnotations;

namespace Users.Application.Users;

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

    /// <summary>
    ///     Validates whether the draft data meets the criteria.
    /// </summary>
    public ValidationException? Validate()
    {
        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();

        if (Validator.TryValidateObject(this, context, results, validateAllProperties: true))
            return null;

        var errors = results.Select(r => r.ErrorMessage).ToList();
        return new ValidationException($"Validation failed: {string.Join(", ", errors)}");
    }
}