using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class ReviewsMockRepository
{
    private readonly List<Review> _reviews;
    private readonly UsersMockRepository _usersRepo;
    private readonly GamesMockRepository _gamesRepo;

    public ReviewsMockRepository(
        UsersMockRepository usersRepo,
        GamesMockRepository gamesRepo)
    {
        _usersRepo = usersRepo;
        _gamesRepo = gamesRepo;

        _reviews = InitializeReviews();
    }

    private List<Review> InitializeReviews()
    {
        var user1 = _usersRepo.GetById(1);
        var user2 = _usersRepo.GetById(2);
        var user3 = _usersRepo.GetById(3);

        var game1 = _gamesRepo.GetById(1);
        var game2 = _gamesRepo.GetById(2);
        var game4 = _gamesRepo.GetById(4);
        var game5 = _gamesRepo.GetById(5);
        var game8 = _gamesRepo.GetById(8);

        var reviews = new List<Review>
        {
            new Review
            {
                Id = 1,
                User = user1,
                Game = game1,
                Score = 95,
                Comment = "Amazing game, incredible story and characters!",
                CreatedAt = new DateTime(2023, 5, 10)
            },
            new Review
            {
                Id = 2,
                User = user2,
                Game = game1,
                Score = 98,
                Comment = "Best RPG I've ever played!",
                CreatedAt = new DateTime(2023, 6, 15)
            },
            new Review
            {
                Id = 3,
                User = user1,
                Game = game4,
                Score = 90,
                Comment = "Great open-world gameplay",
                CreatedAt = new DateTime(2023, 7, 20)
            },
            new Review
            {
                Id = 4,
                User = user3,
                Game = game8,
                Score = 85,
                Comment = "Competitive and fun",
                CreatedAt = new DateTime(2023, 8, 25)
            },
            new Review
            {
                Id = 5,
                User = user2,
                Game = game2,
                Score = 75,
                Comment = "Good game, but buggy at launch",
                CreatedAt = new DateTime(2023, 9, 1)
            },
            new Review
            {
                Id = 6,
                User = user3,
                Game = game5,
                Score = 96,
                Comment = "Outstanding story and immersion",
                CreatedAt = new DateTime(2023, 10, 5)
            }
        };

        // Dodaj recenzije u igre
        if (game1 != null)
        {
            game1.Reviews.Add(reviews[0]);
            game1.Reviews.Add(reviews[1]);
        }
        if (game4 != null)
            game4.Reviews.Add(reviews[2]);
        if (game8 != null)
            game8.Reviews.Add(reviews[3]);
        if (game2 != null)
            game2.Reviews.Add(reviews[4]);
        if (game5 != null)
            game5.Reviews.Add(reviews[5]);

        return reviews;
    }

    public List<Review> GetAll() => _reviews;

    public Review? GetById(int id) => _reviews.FirstOrDefault(r => r.Id == id);
}
