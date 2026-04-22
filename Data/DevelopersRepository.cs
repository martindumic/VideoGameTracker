using Microsoft.EntityFrameworkCore;
using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class DevelopersRepository
{
    private readonly VideoGameTrackerDbContext _dbContext;

    public DevelopersRepository(VideoGameTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Developer> GetAll() => _dbContext.Developers
        .Include(d => d.Games)
        .ToList();

    public Developer? GetById(int id) => _dbContext.Developers
        .Include(d => d.Games)
        .FirstOrDefault(d => d.Id == id);
}
