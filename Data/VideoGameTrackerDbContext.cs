using Microsoft.EntityFrameworkCore;
using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class VideoGameTrackerDbContext : DbContext
{
    public VideoGameTrackerDbContext(DbContextOptions<VideoGameTrackerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Developer> Developers => Set<Developer>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<GameEntry> GameEntries => Set<GameEntry>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Platform> Platforms => Set<Platform>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Game>()
            .HasOne(g => g.Developer)
            .WithMany(d => d.Games)
            .HasForeignKey(g => g.DeveloperId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Game)
            .WithMany(g => g.Reviews)
            .HasForeignKey(r => r.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GameEntry>()
            .HasOne(ge => ge.Game)
            .WithMany(g => g.GameEntries)
            .HasForeignKey(ge => ge.GameId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GameEntry>()
            .HasOne(ge => ge.User)
            .WithMany(u => u.GameEntries)
            .HasForeignKey(ge => ge.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GameEntry>()
            .HasOne(ge => ge.Review)
            .WithOne()
            .HasForeignKey<GameEntry>(ge => ge.ReviewId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Game>()
            .HasMany(g => g.Genres)
            .WithMany(g => g.Games)
            .UsingEntity("GameGenres");

        modelBuilder.Entity<Game>()
            .HasMany(g => g.Platforms)
            .WithMany(p => p.Games)
            .UsingEntity("GamePlatforms");

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Developer>().HasData(
            new Developer { Id = 1, Name = "CD Projekt Red", Country = "Poland", Founded = new DateTime(2002, 5, 30), Description = "Polish video game developer known for The Witcher series" },
            new Developer { Id = 2, Name = "Rockstar Games", Country = "USA", Founded = new DateTime(1998, 12, 1), Description = "American video game developer and publisher known for GTA series" },
            new Developer { Id = 3, Name = "Valve Corporation", Country = "USA", Founded = new DateTime(1996, 8, 1), Description = "Developer of legendary fps games and Half-Life series" }
        );

        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = 1, Name = "Role-Playing Game", Description = "Games with character progression and storytelling" },
            new Genre { Id = 2, Name = "Action", Description = "Fast-paced action-oriented games" },
            new Genre { Id = 3, Name = "First-Person Shooter", Description = "Fast shooter games played from first-person perspective" },
            new Genre { Id = 4, Name = "Adventure", Description = "Adventure and exploration games" }
        );

        modelBuilder.Entity<Platform>().HasData(
            new Platform { Id = 1, Name = "PC", Type = PlatformType.PC },
            new Platform { Id = 2, Name = "PlayStation 5", Type = PlatformType.PlayStation },
            new Platform { Id = 3, Name = "Xbox Series X", Type = PlatformType.Xbox }
        );

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "GamerPro", Email = "gamerpro@email.com", Password = "hashed_password_1", RegisteredAt = new DateTime(2020, 3, 15) },
            new User { Id = 2, Username = "RPGFanatic", Email = "rpgfan@email.com", Password = "hashed_password_2", RegisteredAt = new DateTime(2021, 7, 22) },
            new User { Id = 3, Username = "ActionJunkie", Email = "actionjunkie@email.com", Password = "hashed_password_3", RegisteredAt = new DateTime(2019, 11, 10) }
        );

        modelBuilder.Entity<Game>().HasData(
            new Game { Id = 1, Title = "The Witcher 3: Wild Hunt", ReleaseYear = 2015, Description = "Open-world RPG with rich storytelling and complex characters", DeveloperId = 1, AverageRating = 95 },
            new Game { Id = 2, Title = "Cyberpunk 2077", ReleaseYear = 2020, Description = "Futuristic action RPG set in Night City", DeveloperId = 1, AverageRating = 77 },
            new Game { Id = 3, Title = "The Witcher 2: Assassins of Kings", ReleaseYear = 2011, Description = "Predecessor to The Witcher 3", DeveloperId = 1, AverageRating = 88 },
            new Game { Id = 4, Title = "Grand Theft Auto V", ReleaseYear = 2013, Description = "Open-world action game set in Los Santos", DeveloperId = 2, AverageRating = 97 },
            new Game { Id = 5, Title = "Red Dead Redemption 2", ReleaseYear = 2018, Description = "Western action-adventure game", DeveloperId = 2, AverageRating = 97 },
            new Game { Id = 6, Title = "GTA IV", ReleaseYear = 2008, Description = "Action game set in Liberty City", DeveloperId = 2, AverageRating = 94 },
            new Game { Id = 7, Title = "Half-Life 2", ReleaseYear = 2004, Description = "Revolutionary first-person shooter", DeveloperId = 3, AverageRating = 96 },
            new Game { Id = 8, Title = "Counter-Strike 2", ReleaseYear = 2023, Description = "Competitive team-based FPS", DeveloperId = 3, AverageRating = 82 },
            new Game { Id = 9, Title = "Half-Life: Alyx", ReleaseYear = 2020, Description = "VR prequel to Half-Life 2", DeveloperId = 3, AverageRating = 93 }
        );

        modelBuilder.Entity<Review>().HasData(
            new Review { Id = 1, UserId = 1, GameId = 1, Score = 95, Comment = "Amazing game, incredible story and characters!", CreatedAt = new DateTime(2023, 5, 10) },
            new Review { Id = 2, UserId = 2, GameId = 1, Score = 98, Comment = "Best RPG I've ever played!", CreatedAt = new DateTime(2023, 6, 15) },
            new Review { Id = 3, UserId = 1, GameId = 4, Score = 90, Comment = "Great open-world gameplay", CreatedAt = new DateTime(2023, 7, 20) },
            new Review { Id = 4, UserId = 3, GameId = 8, Score = 85, Comment = "Competitive and fun", CreatedAt = new DateTime(2023, 8, 25) },
            new Review { Id = 5, UserId = 2, GameId = 2, Score = 75, Comment = "Good game, but buggy at launch", CreatedAt = new DateTime(2023, 9, 1) },
            new Review { Id = 6, UserId = 3, GameId = 5, Score = 96, Comment = "Outstanding story and immersion", CreatedAt = new DateTime(2023, 10, 5) }
        );

        modelBuilder.Entity<GameEntry>().HasData(
            new GameEntry { Id = 1, GameId = 1, UserId = 1, Status = GameStatus.Finished, DateAdded = new DateTime(2022, 3, 10), HoursPlayed = 150, ReviewId = 1 },
            new GameEntry { Id = 2, GameId = 1, UserId = 2, Status = GameStatus.Finished, DateAdded = new DateTime(2022, 5, 15), HoursPlayed = 200, ReviewId = 2 },
            new GameEntry { Id = 3, GameId = 4, UserId = 1, Status = GameStatus.Playing, DateAdded = new DateTime(2023, 6, 20), HoursPlayed = 80, ReviewId = 3 },
            new GameEntry { Id = 4, GameId = 8, UserId = 3, Status = GameStatus.Playing, DateAdded = new DateTime(2023, 9, 10), HoursPlayed = 45, ReviewId = 4 },
            new GameEntry { Id = 5, GameId = 2, UserId = 2, Status = GameStatus.Finished, DateAdded = new DateTime(2021, 1, 5), HoursPlayed = 120, ReviewId = 5 },
            new GameEntry { Id = 6, GameId = 5, UserId = 3, Status = GameStatus.Finished, DateAdded = new DateTime(2022, 8, 15), HoursPlayed = 180, ReviewId = 6 },
            new GameEntry { Id = 7, GameId = 7, UserId = 3, Status = GameStatus.Planned, DateAdded = new DateTime(2023, 11, 1), HoursPlayed = 0, ReviewId = null },
            new GameEntry { Id = 8, GameId = 3, UserId = 2, Status = GameStatus.Finished, DateAdded = new DateTime(2020, 10, 20), HoursPlayed = 95, ReviewId = null }
        );

        modelBuilder.Entity("GameGenres").HasData(
            new { GamesId = 1, GenresId = 1 },
            new { GamesId = 1, GenresId = 2 },
            new { GamesId = 1, GenresId = 4 },
            new { GamesId = 2, GenresId = 1 },
            new { GamesId = 2, GenresId = 2 },
            new { GamesId = 3, GenresId = 1 },
            new { GamesId = 3, GenresId = 2 },
            new { GamesId = 4, GenresId = 2 },
            new { GamesId = 4, GenresId = 4 },
            new { GamesId = 5, GenresId = 2 },
            new { GamesId = 5, GenresId = 4 },
            new { GamesId = 6, GenresId = 2 },
            new { GamesId = 7, GenresId = 3 },
            new { GamesId = 7, GenresId = 2 },
            new { GamesId = 8, GenresId = 3 },
            new { GamesId = 9, GenresId = 3 },
            new { GamesId = 9, GenresId = 2 }
        );

        modelBuilder.Entity("GamePlatforms").HasData(
            new { GamesId = 1, PlatformsId = 1 },
            new { GamesId = 1, PlatformsId = 2 },
            new { GamesId = 1, PlatformsId = 3 },
            new { GamesId = 2, PlatformsId = 1 },
            new { GamesId = 2, PlatformsId = 2 },
            new { GamesId = 2, PlatformsId = 3 },
            new { GamesId = 3, PlatformsId = 1 },
            new { GamesId = 4, PlatformsId = 1 },
            new { GamesId = 4, PlatformsId = 2 },
            new { GamesId = 4, PlatformsId = 3 },
            new { GamesId = 5, PlatformsId = 1 },
            new { GamesId = 5, PlatformsId = 2 },
            new { GamesId = 5, PlatformsId = 3 },
            new { GamesId = 6, PlatformsId = 1 },
            new { GamesId = 6, PlatformsId = 3 },
            new { GamesId = 7, PlatformsId = 1 },
            new { GamesId = 8, PlatformsId = 1 },
            new { GamesId = 9, PlatformsId = 1 }
        );
    }
}
