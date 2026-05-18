using System.ComponentModel.DataAnnotations;

namespace VideoGameTracker.ViewModels;

public class DeveloperFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(200, ErrorMessage = "Name can be up to 200 characters.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Country is required.")]
    [StringLength(100, ErrorMessage = "Country can be up to 100 characters.")]
    public string? Country { get; set; }

    [Required(ErrorMessage = "Founded date is required.")]
    public string? Founded { get; set; }

    [StringLength(2000, ErrorMessage = "Description can be up to 2000 characters.")]
    public string? Description { get; set; }
}
