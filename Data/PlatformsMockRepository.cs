using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class PlatformsMockRepository
{
    private readonly List<Platform> _platforms;

    public PlatformsMockRepository()
    {
        _platforms = new List<Platform>
        {
            new Platform
            {
                Id = 1,
                Name = "PC",
                Type = PlatformType.PC,
                Games = new List<Game>()
            },
            new Platform
            {
                Id = 2,
                Name = "PlayStation 5",
                Type = PlatformType.PlayStation,
                Games = new List<Game>()
            },
            new Platform
            {
                Id = 3,
                Name = "Xbox Series X",
                Type = PlatformType.Xbox,
                Games = new List<Game>()
            }
        };
    }

    public List<Platform> GetAll() => _platforms;

    public Platform? GetById(int id) => _platforms.FirstOrDefault(p => p.Id == id);
}
