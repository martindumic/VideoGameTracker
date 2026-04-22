using System.ComponentModel.DataAnnotations;

namespace VideoGameTracker.Models;

public class Developer
{
    [Key]
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Country { get; set; }

    public DateTime Founded { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Game> Games { get; set; }

    public Developer()
    {
        Games = new List<Game>();
    }
}
