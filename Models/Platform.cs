using System.ComponentModel.DataAnnotations;

namespace VideoGameTracker.Models;

public class Platform
{
    [Key]
    public int Id { get; set; }

    public string? Name { get; set; }

    public PlatformType Type { get; set; }

    public virtual ICollection<Game> Games { get; set; }

    public Platform()
    {
        Games = new List<Game>();
    }
}
