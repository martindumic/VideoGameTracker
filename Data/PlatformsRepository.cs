using Microsoft.EntityFrameworkCore;
using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class PlatformsRepository
{
    private readonly VideoGameTrackerDbContext _dbContext;

    public PlatformsRepository(VideoGameTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Platform> GetAll() => _dbContext.Platforms
        .Include(p => p.Games)
        .ToList();

    public Platform? GetById(int id) => _dbContext.Platforms
        .Include(p => p.Games)
        .FirstOrDefault(p => p.Id == id);
}
