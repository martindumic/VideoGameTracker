using Microsoft.EntityFrameworkCore;
using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class GenresRepository : IGenresRepository
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

    public List<Genre> Search(string? query)
    {
        var genresQuery = _dbContext.Genres
            .Include(g => g.Games)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = $"%{query.Trim()}%";
            genresQuery = genresQuery.Where(g =>
                EF.Functions.Like(g.Name ?? string.Empty, term) ||
                EF.Functions.Like(g.Description ?? string.Empty, term)
            );
        }

        return genresQuery.ToList();
    }

    public void Create(Genre genre)
    {
        _dbContext.Genres.Add(genre);
        _dbContext.SaveChanges();
    }

    public bool Update(Genre genre)
    {
        var existing = _dbContext.Genres.FirstOrDefault(g => g.Id == genre.Id);
        if (existing == null)
        {
            return false;
        }

        _dbContext.Entry(existing).CurrentValues.SetValues(genre);
        _dbContext.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var genre = _dbContext.Genres.FirstOrDefault(g => g.Id == id);
        if (genre == null)
        {
            return false;
        }

        _dbContext.Genres.Remove(genre);
        _dbContext.SaveChanges();
        return true;
    }
}
