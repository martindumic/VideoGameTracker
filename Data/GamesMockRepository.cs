using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class GamesMockRepository
{
    private readonly List<Game> _games;
    private readonly DevelopersMockRepository _developersRepo;
    private readonly GenresMockRepository _genresRepo;
    private readonly PlatformsMockRepository _platformsRepo;

    public GamesMockRepository(
        DevelopersMockRepository developersRepo,
        GenresMockRepository genresRepo,
        PlatformsMockRepository platformsRepo)
    {
        _developersRepo = developersRepo;
        _genresRepo = genresRepo;
        _platformsRepo = platformsRepo;

        _games = InitializeGames();
    }

    private List<Game> InitializeGames()
    {
        var developer1 = _developersRepo.GetById(1);
        var developer2 = _developersRepo.GetById(2);
        var developer3 = _developersRepo.GetById(3);

        var genreRPG = _genresRepo.GetById(1);
        var genreAction = _genresRepo.GetById(2);
        var genreFPS = _genresRepo.GetById(3);
        var genreAdventure = _genresRepo.GetById(4);

        var platformPC = _platformsRepo.GetById(1);
        var platformPS5 = _platformsRepo.GetById(2);
        var platformXbox = _platformsRepo.GetById(3);

        var games = new List<Game>
        {
            new Game
            {
                Id = 1,
                Title = "The Witcher 3: Wild Hunt",
                ReleaseYear = 2015,
                Description = "Open-world RPG with rich storytelling and complex characters",
                Developer = developer1,
                AverageRating = 95,
                Genres = new List<Genre> { genreRPG, genreAction, genreAdventure },
                Platforms = new List<Platform> { platformPC, platformPS5, platformXbox },
                Reviews = new List<Review>()
            },
            new Game
            {
                Id = 2,
                Title = "Cyberpunk 2077",
                ReleaseYear = 2020,
                Description = "Futuristic action RPG set in Night City",
                Developer = developer1,
                AverageRating = 77,
                Genres = new List<Genre> { genreRPG, genreAction },
                Platforms = new List<Platform> { platformPC, platformPS5, platformXbox },
                Reviews = new List<Review>()
            },
            new Game
            {
                Id = 3,
                Title = "The Witcher 2: Assassins of Kings",
                ReleaseYear = 2011,
                Description = "Predecessor to The Witcher 3",
                Developer = developer1,
                AverageRating = 88,
                Genres = new List<Genre> { genreRPG, genreAction },
                Platforms = new List<Platform> { platformPC },
                Reviews = new List<Review>()
            },
            new Game
            {
                Id = 4,
                Title = "Grand Theft Auto V",
                ReleaseYear = 2013,
                Description = "Open-world action game set in Los Santos",
                Developer = developer2,
                AverageRating = 97,
                Genres = new List<Genre> { genreAction, genreAdventure },
                Platforms = new List<Platform> { platformPC, platformPS5, platformXbox },
                Reviews = new List<Review>()
            },
            new Game
            {
                Id = 5,
                Title = "Red Dead Redemption 2",
                ReleaseYear = 2018,
                Description = "Western action-adventure game",
                Developer = developer2,
                AverageRating = 97,
                Genres = new List<Genre> { genreAction, genreAdventure },
                Platforms = new List<Platform> { platformPC, platformPS5, platformXbox },
                Reviews = new List<Review>()
            },
            new Game
            {
                Id = 6,
                Title = "GTA IV",
                ReleaseYear = 2008,
                Description = "Action game set in Liberty City",
                Developer = developer2,
                AverageRating = 94,
                Genres = new List<Genre> { genreAction },
                Platforms = new List<Platform> { platformPC, platformXbox },
                Reviews = new List<Review>()
            },
            new Game
            {
                Id = 7,
                Title = "Half-Life 2",
                ReleaseYear = 2004,
                Description = "Revolutionary first-person shooter",
                Developer = developer3,
                AverageRating = 96,
                Genres = new List<Genre> { genreFPS, genreAction },
                Platforms = new List<Platform> { platformPC },
                Reviews = new List<Review>()
            },
            new Game
            {
                Id = 8,
                Title = "Counter-Strike 2",
                ReleaseYear = 2023,
                Description = "Competitive team-based FPS",
                Developer = developer3,
                AverageRating = 82,
                Genres = new List<Genre> { genreFPS },
                Platforms = new List<Platform> { platformPC },
                Reviews = new List<Review>()
            },
            new Game
            {
                Id = 9,
                Title = "Half-Life: Alyx",
                ReleaseYear = 2020,
                Description = "VR prequel to Half-Life 2",
                Developer = developer3,
                AverageRating = 93,
                Genres = new List<Genre> { genreFPS, genreAction },
                Platforms = new List<Platform> { platformPC },
                Reviews = new List<Review>()
            }
        };

        // Dodaj igre u developere
        if (developer1 != null)
            developer1.Games.AddRange(games.Where(g => g.Developer?.Id == 1));
        if (developer2 != null)
            developer2.Games.AddRange(games.Where(g => g.Developer?.Id == 2));
        if (developer3 != null)
            developer3.Games.AddRange(games.Where(g => g.Developer?.Id == 3));

        return games;
    }

    public List<Game> GetAll() => _games;

    public Game? GetById(int id) => _games.FirstOrDefault(g => g.Id == id);
}
