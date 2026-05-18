using System.ComponentModel.DataAnnotations;

namespace VideoGameTracker.ViewModels;

public class UserFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(120, ErrorMessage = "Username can be up to 120 characters.")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email must be valid.")]
    [StringLength(200, ErrorMessage = "Email can be up to 200 characters.")]
    public string? Email { get; set; }

    [StringLength(200, ErrorMessage = "Password can be up to 200 characters.")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Registered date is required.")]
    public string? RegisteredAt { get; set; }

    public bool RequirePassword { get; set; }
}
