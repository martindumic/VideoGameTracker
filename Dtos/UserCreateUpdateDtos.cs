using System.ComponentModel.DataAnnotations;

namespace VideoGameTracker.Dtos;

public class UserCreateDto
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 6)]
    public string? Password { get; set; }

    [Required]
    [StringLength(11, MinimumLength = 11)]
    [RegularExpression("^[0-9]*$")]
    public string? OIB { get; set; }

    [Required]
    [StringLength(13, MinimumLength = 13)]
    [RegularExpression("^[0-9]*$")]
    public string? JMBG { get; set; }
}

public class UserUpdateDto
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [StringLength(200)]
    public string? UserName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(11, MinimumLength = 11)]
    [RegularExpression("^[0-9]*$")]
    public string? OIB { get; set; }

    [StringLength(13, MinimumLength = 13)]
    [RegularExpression("^[0-9]*$")]
    public string? JMBG { get; set; }

    [StringLength(200, MinimumLength = 6)]
    public string? NewPassword { get; set; }
}
