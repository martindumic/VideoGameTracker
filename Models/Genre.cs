using System.ComponentModel.DataAnnotations;

namespace VideoGameTracker.Models;

public class Genre
{
    [Key]
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Game> Games { get; set; }

    public Genre()
    {
        Games = new List<Game>();
    }
}
