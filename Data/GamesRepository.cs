using Microsoft.EntityFrameworkCore;
using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class GamesRepository
{
    private readonly VideoGameTrackerDbContext _dbContext;

    public GamesRepository(VideoGameTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Game> GetAll() => _dbContext.Games
        .Include(g => g.Developer)
        .Include(g => g.Platforms)
        .ToList();

    public Game? GetById(int id) => _dbContext.Games
        .Include(g => g.Developer)
        .Include(g => g.Genres)
        .Include(g => g.Platforms)
        .FirstOrDefault(g => g.Id == id);
}
