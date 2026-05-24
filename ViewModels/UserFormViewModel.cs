using System.ComponentModel.DataAnnotations;

namespace VideoGameTracker.ViewModels;

public class UserFormViewModel
{
    public string? Id { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(120, ErrorMessage = "Username can be up to 120 characters.")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email must be valid.")]
    [StringLength(200, ErrorMessage = "Email can be up to 200 characters.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "OIB is required.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "OIB must be 11 digits.")]
    [RegularExpression("^[0-9]*$", ErrorMessage = "OIB must contain only digits.")]
    public string? OIB { get; set; }

    [Required(ErrorMessage = "JMBG is required.")]
    [StringLength(13, MinimumLength = 13, ErrorMessage = "JMBG must be 13 digits.")]
    [RegularExpression("^[0-9]*$", ErrorMessage = "JMBG must contain only digits.")]
    public string? JMBG { get; set; }

    [StringLength(200, ErrorMessage = "Password can be up to 200 characters.")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Registered date is required.")]
    public string? RegisteredAt { get; set; }

    public bool RequirePassword { get; set; }
}
