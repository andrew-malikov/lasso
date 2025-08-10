using System.ComponentModel.DataAnnotations;

namespace Users.WebApi.Domain;

public record LoginRequest
{
    [Required(AllowEmptyStrings = false)]
    [MinLength(3), MaxLength(60)]
    public required string Username { get; init; }

    [Required(AllowEmptyStrings = false)]
    [MinLength(8), MaxLength(40)]
    public required string Password { get; init; }
}