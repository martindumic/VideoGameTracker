using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class GenresMockRepository
{
    private readonly List<Genre> _genres;

    public GenresMockRepository()
    {
        _genres = new List<Genre>
        {
            new Genre
            {
                Id = 1,
                Name = "Role-Playing Game",
                Description = "Games with character progression and storytelling",
                Games = new List<Game>()
            },
            new Genre
            {
                Id = 2,
                Name = "Action",
                Description = "Fast-paced action-oriented games",
                Games = new List<Game>()
            },
            new Genre
            {
                Id = 3,
                Name = "First-Person Shooter",
                Description = "Fast shooter games played from first-person perspective",
                Games = new List<Game>()
            },
            new Genre
            {
                Id = 4,
                Name = "Adventure",
                Description = "Adventure and exploration games",
                Games = new List<Game>()
            }
        };
    }

    public List<Genre> GetAll() => _genres;

    public Genre? GetById(int id) => _genres.FirstOrDefault(g => g.Id == id);
}
