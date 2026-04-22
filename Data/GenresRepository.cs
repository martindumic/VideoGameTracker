using Microsoft.EntityFrameworkCore;
using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class GenresRepository
{
    private readonly VideoGameTrackerDbContext _dbContext;

    public GenresRepository(VideoGameTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Genre> GetAll() => _dbContext.Genres
        .Include(g => g.Games)
        .ToList();

    public Genre? GetById(int id) => _dbContext.Genres
        .Include(g => g.Games)
        .FirstOrDefault(g => g.Id == id);
}
