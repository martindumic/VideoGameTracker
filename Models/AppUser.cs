using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace VideoGameTracker.Models;

public class AppUser : IdentityUser
{
    [Required]
    [StringLength(11, MinimumLength = 11)]
    [RegularExpression("^[0-9]*$")]
    public string OIB { get; set; } = string.Empty;

    [Required]
    [StringLength(13, MinimumLength = 13)]
    [RegularExpression("^[0-9]*$")]
    public string JMBG { get; set; } = string.Empty;

    public DateTime RegisteredAt { get; set; }

    public virtual ICollection<GameEntry> GameEntries { get; set; } = new List<GameEntry>();
}
