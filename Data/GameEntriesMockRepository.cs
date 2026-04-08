using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class GameEntriesMockRepository
{
    private readonly List<GameEntry> _gameEntries;
    private readonly GamesMockRepository _gamesRepo;
    private readonly UsersMockRepository _usersRepo;
    private readonly ReviewsMockRepository _reviewsRepo;

    public GameEntriesMockRepository(
        GamesMockRepository gamesRepo,
        UsersMockRepository usersRepo,
        ReviewsMockRepository reviewsRepo)
    {
        _gamesRepo = gamesRepo;
        _usersRepo = usersRepo;
        _reviewsRepo = reviewsRepo;

        _gameEntries = InitializeGameEntries();
    }

    private List<GameEntry> InitializeGameEntries()
    {
        var game1 = _gamesRepo.GetById(1);
        var game2 = _gamesRepo.GetById(2);
        var game3 = _gamesRepo.GetById(3);
        var game4 = _gamesRepo.GetById(4);
        var game5 = _gamesRepo.GetById(5);
        var game7 = _gamesRepo.GetById(7);
        var game8 = _gamesRepo.GetById(8);

        var user1 = _usersRepo.GetById(1);
        var user2 = _usersRepo.GetById(2);
        var user3 = _usersRepo.GetById(3);

        var review1 = _reviewsRepo.GetById(1);
        var review2 = _reviewsRepo.GetById(2);
        var review3 = _reviewsRepo.GetById(3);
        var review4 = _reviewsRepo.GetById(4);
        var review5 = _reviewsRepo.GetById(5);
        var review6 = _reviewsRepo.GetById(6);

        var entries = new List<GameEntry>
        {
            new GameEntry
            {
                Id = 1,
                Game = game1,
                User = user1,
                Status = GameStatus.Finished,
                DateAdded = new DateTime(2022, 3, 10),
                HoursPlayed = 150,
                Review = review1
            },
            new GameEntry
            {
                Id = 2,
                Game = game1,
                User = user2,
                Status = GameStatus.Finished,
                DateAdded = new DateTime(2022, 5, 15),
                HoursPlayed = 200,
                Review = review2
            },
            new GameEntry
            {
                Id = 3,
                Game = game4,
                User = user1,
                Status = GameStatus.Playing,
                DateAdded = new DateTime(2023, 6, 20),
                HoursPlayed = 80,
                Review = review3
            },
            new GameEntry
            {
                Id = 4,
                Game = game8,
                User = user3,
                Status = GameStatus.Playing,
                DateAdded = new DateTime(2023, 9, 10),
                HoursPlayed = 45,
                Review = review4
            },
            new GameEntry
            {
                Id = 5,
                Game = game2,
                User = user2,
                Status = GameStatus.Finished,
                DateAdded = new DateTime(2021, 1, 5),
                HoursPlayed = 120,
                Review = review5
            },
            new GameEntry
            {
                Id = 6,
                Game = game5,
                User = user3,
                Status = GameStatus.Finished,
                DateAdded = new DateTime(2022, 8, 15),
                HoursPlayed = 180,
                Review = review6
            },
            new GameEntry
            {
                Id = 7,
                Game = game7,
                User = user3,
                Status = GameStatus.Planned,
                DateAdded = new DateTime(2023, 11, 1),
                HoursPlayed = 0,
                Review = null
            },
            new GameEntry
            {
                Id = 8,
                Game = game3,
                User = user2,
                Status = GameStatus.Finished,
                DateAdded = new DateTime(2020, 10, 20),
                HoursPlayed = 95,
                Review = null
            }
        };

        // Dodaj GameEntry-je u korisnike
        if (user1 != null)
        {
            user1.GameEntries.Add(entries[0]);
            user1.GameEntries.Add(entries[2]);
        }
        if (user2 != null)
        {
            user2.GameEntries.Add(entries[1]);
            user2.GameEntries.Add(entries[4]);
            user2.GameEntries.Add(entries[7]);
        }
        if (user3 != null)
        {
            user3.GameEntries.Add(entries[3]);
            user3.GameEntries.Add(entries[5]);
            user3.GameEntries.Add(entries[6]);
        }

        return entries;
    }

    public List<GameEntry> GetAll() => _gameEntries;

    public GameEntry? GetById(int id) => _gameEntries.FirstOrDefault(e => e.Id == id);
}
