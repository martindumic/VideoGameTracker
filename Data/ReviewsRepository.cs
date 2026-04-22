using Microsoft.EntityFrameworkCore;
using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class ReviewsRepository
{
    private readonly VideoGameTrackerDbContext _dbContext;

    public ReviewsRepository(VideoGameTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Review> GetAll() => _dbContext.Reviews
        .Include(r => r.Game)
        .Include(r => r.User)
        .ToList();

    public Review? GetById(int id) => _dbContext.Reviews
        .Include(r => r.Game)
        .Include(r => r.User)
        .FirstOrDefault(r => r.Id == id);

    public void Add(Review review)
    {
        _dbContext.Reviews.Add(review);
        _dbContext.SaveChanges();
    }
}
