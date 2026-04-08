using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class DevelopersMockRepository
{
    private readonly List<Developer> _developers;

    public DevelopersMockRepository()
    {
        _developers = new List<Developer>
        {
            new Developer
            {
                Id = 1,
                Name = "CD Projekt Red",
                Country = "Poland",
                Founded = new DateTime(2002, 5, 30),
                Description = "Polish video game developer known for The Witcher series",
                Games = new List<Game>()
            },
            new Developer
            {
                Id = 2,
                Name = "Rockstar Games",
                Country = "USA",
                Founded = new DateTime(1998, 12, 1),
                Description = "American video game developer and publisher known for GTA series",
                Games = new List<Game>()
            },
            new Developer
            {
                Id = 3,
                Name = "Valve Corporation",
                Country = "USA",
                Founded = new DateTime(1996, 8, 1),
                Description = "Developer of legendary fps games and Half-Life series",
                Games = new List<Game>()
            }
        };
    }

    public List<Developer> GetAll() => _developers;

    public Developer? GetById(int id) => _developers.FirstOrDefault(d => d.Id == id);
}
