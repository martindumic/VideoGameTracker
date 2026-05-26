using VideoGameTracker.Data;
using VideoGameTracker.Models;

namespace VideoGameTracker.Tests;

public static class TestDataSeeder
{
    public static Developer SeedDeveloper(VideoGameTrackerDbContext db, string name = "Test Studio")
    {
        var developer = new Developer
        {
            Name = name,
            Country = "Testland",
            Founded = new DateTime(2000, 1, 1),
            Description = "Test developer"
        };

        db.Developers.Add(developer);
        db.SaveChanges();
        return developer;
    }

    public static Genre SeedGenre(VideoGameTrackerDbContext db, string name = "Test Genre")
    {
        var genre = new Genre { Name = name, Description = "Test genre" };
        db.Genres.Add(genre);
        db.SaveChanges();
        return genre;
    }

    public static Platform SeedPlatform(VideoGameTrackerDbContext db, string name = "Test Platform")
    {
        var platform = new Platform { Name = name, Type = PlatformType.PC };
        db.Platforms.Add(platform);
        db.SaveChanges();
        return platform;
    }

    public static Game SeedGame(VideoGameTrackerDbContext db, Developer developer, List<Genre> genres, List<Platform> platforms)
    {
        var game = new Game
        {
            Title = "Test Game",
            ReleaseYear = 2020,
            Description = "Test description",
            Developer = developer,
            AverageRating = 91,
            Genres = genres,
            Platforms = platforms
        };

        db.Games.Add(game);
        db.SaveChanges();
        return game;
    }

    public static int SeedGameWithRelations(VideoGameTrackerDbContext db)
    {
        var developer = SeedDeveloper(db);
        var genre = SeedGenre(db);
        var platform = SeedPlatform(db);
        var game = SeedGame(db, developer, new List<Genre> { genre }, new List<Platform> { platform });
        return game.Id;
    }

    public static AppUser SeedUser(VideoGameTrackerDbContext db, string id, string email, string userName)
    {
        var user = new AppUser
        {
            Id = id,
            UserName = userName,
            Email = email,
            OIB = "12345678901",
            JMBG = "1234567890123",
            RegisteredAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        db.SaveChanges();
        return user;
    }

    public static GameEntry SeedGameEntry(VideoGameTrackerDbContext db, int gameId, string userId, GameStatus status)
    {
        var entry = new GameEntry
        {
            GameId = gameId,
            UserId = userId,
            Status = status,
            HoursPlayed = 12,
            ReviewScore = 85,
            ReviewComment = "Test review",
            CreatedAt = DateTime.UtcNow
        };

        db.GameEntries.Add(entry);
        db.SaveChanges();
        return entry;
    }
}
