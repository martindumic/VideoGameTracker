using VideoGameTracker.Models;

// ============= INICIJALIZACIJA PODATAKA =============

// 1. Kreiranje 3 RAZGRANATA DEVELOPERA
var developer1 = new Developer
{
    Id = 1,
    Name = "CD Projekt Red",
    Country = "Poland",
    Founded = new DateTime(2002, 5, 30),
    Description = "Polish video game developer known for The Witcher series",
    Games = new List<Game>()
};

var developer2 = new Developer
{
    Id = 2,
    Name = "Rockstar Games",
    Country = "USA",
    Founded = new DateTime(1998, 12, 1),
    Description = "American video game developer and publisher known for GTA series",
    Games = new List<Game>()
};

var developer3 = new Developer
{
    Id = 3,
    Name = "Valve Corporation",
    Country = "USA",
    Founded = new DateTime(1996, 8, 1),
    Description = "Developer of legendary fps games and Half-Life series",
    Games = new List<Game>()
};

// 2. Kreiranje žanrova
var genreRPG = new Genre
{
    Id = 1,
    Name = "Role-Playing Game",
    Description = "Games with character progression and storytelling",
    Games = new List<Game>()
};

var genreAction = new Genre
{
    Id = 2,
    Name = "Action",
    Description = "Fast-paced action-oriented games",
    Games = new List<Game>()
};

var genreFPS = new Genre
{
    Id = 3,
    Name = "First-Person Shooter",
    Description = "Fast shooter games played from first-person perspective",
    Games = new List<Game>()
};

var genreAdventure = new Genre
{
    Id = 4,
    Name = "Adventure",
    Description = "Adventure and exploration games",
    Games = new List<Game>()
};

// 3. Kreiranje platformi
var platformPC = new Platform
{
    Id = 1,
    Name = "PC",
    Type = PlatformType.PC,
    Games = new List<Game>()
};

var platformPS5 = new Platform
{
    Id = 2,
    Name = "PlayStation 5",
    Type = PlatformType.PlayStation,
    Games = new List<Game>()
};

var platformXbox = new Platform
{
    Id = 3,
    Name = "Xbox Series X",
    Type = PlatformType.Xbox,
    Games = new List<Game>()
};

// 4. Kreiranje igara za Developer 1 (CD Projekt Red)
var game1 = new Game
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
};

var game2 = new Game
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
};

var game3 = new Game
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
};

developer1.Games.AddRange(new[] { game1, game2, game3 });

// 5. Kreiranje igara za Developer 2 (Rockstar Games)
var game4 = new Game
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
};

var game5 = new Game
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
};

var game6 = new Game
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
};

developer2.Games.AddRange(new[] { game4, game5, game6 });

// 6. Kreiranje igara za Developer 3 (Valve)
var game7 = new Game
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
};

var game8 = new Game
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
};

var game9 = new Game
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
};

developer3.Games.AddRange(new[] { game7, game8, game9 });

// 7. Kreiranje korisnika
var user1 = new User
{
    Id = 1,
    Username = "GamerPro",
    Email = "gamerpro@email.com",
    Password = "hashed_password_1",
    RegisteredAt = new DateTime(2020, 3, 15),
    GameEntries = new List<GameEntry>()
};

var user2 = new User
{
    Id = 2,
    Username = "RPGFanatic",
    Email = "rpgfan@email.com",
    Password = "hashed_password_2",
    RegisteredAt = new DateTime(2021, 7, 22),
    GameEntries = new List<GameEntry>()
};

var user3 = new User
{
    Id = 3,
    Username = "ActionJunkie",
    Email = "actionjunkie@email.com",
    Password = "hashed_password_3",
    RegisteredAt = new DateTime(2019, 11, 10),
    GameEntries = new List<GameEntry>()
};

// 8. Kreiranje recenzija
var review1 = new Review
{
    Id = 1,
    User = user1,
    Game = game1,
    Score = 95,
    Comment = "Amazing game, incredible story and characters!",
    CreatedAt = new DateTime(2023, 5, 10)
};

var review2 = new Review
{
    Id = 2,
    User = user2,
    Game = game1,
    Score = 98,
    Comment = "Best RPG I've ever played!",
    CreatedAt = new DateTime(2023, 6, 15)
};

var review3 = new Review
{
    Id = 3,
    User = user1,
    Game = game4,
    Score = 90,
    Comment = "Great open-world gameplay",
    CreatedAt = new DateTime(2023, 7, 20)
};

var review4 = new Review
{
    Id = 4,
    User = user3,
    Game = game8,
    Score = 85,
    Comment = "Competitive and fun",
    CreatedAt = new DateTime(2023, 8, 25)
};

var review5 = new Review
{
    Id = 5,
    User = user2,
    Game = game2,
    Score = 75,
    Comment = "Good game, but buggy at launch",
    CreatedAt = new DateTime(2023, 9, 1)
};

var review6 = new Review
{
    Id = 6,
    User = user3,
    Game = game5,
    Score = 96,
    Comment = "Outstanding story and immersion",
    CreatedAt = new DateTime(2023, 10, 5)
};

game1.Reviews.AddRange(new[] { review1, review2 });
game4.Reviews.Add(review3);
game8.Reviews.Add(review4);
game2.Reviews.Add(review5);
game5.Reviews.Add(review6);

// 9. Kreiranje GameEntry-ja (korisnički unosi igara)
var entry1 = new GameEntry
{
    Id = 1,
    Game = game1,
    User = user1,
    Status = GameStatus.Finished,
    DateAdded = new DateTime(2022, 3, 10),
    HoursPlayed = 150,
    Review = review1
};

var entry2 = new GameEntry
{
    Id = 2,
    Game = game1,
    User = user2,
    Status = GameStatus.Finished,
    DateAdded = new DateTime(2022, 5, 15),
    HoursPlayed = 200,
    Review = review2
};

var entry3 = new GameEntry
{
    Id = 3,
    Game = game4,
    User = user1,
    Status = GameStatus.Playing,
    DateAdded = new DateTime(2023, 6, 20),
    HoursPlayed = 80,
    Review = review3
};

var entry4 = new GameEntry
{
    Id = 4,
    Game = game8,
    User = user3,
    Status = GameStatus.Playing,
    DateAdded = new DateTime(2023, 9, 10),
    HoursPlayed = 45,
    Review = review4
};

var entry5 = new GameEntry
{
    Id = 5,
    Game = game2,
    User = user2,
    Status = GameStatus.Finished,
    DateAdded = new DateTime(2021, 1, 5),
    HoursPlayed = 120,
    Review = review5
};

var entry6 = new GameEntry
{
    Id = 6,
    Game = game5,
    User = user3,
    Status = GameStatus.Finished,
    DateAdded = new DateTime(2022, 8, 15),
    HoursPlayed = 180,
    Review = review6
};

var entry7 = new GameEntry
{
    Id = 7,
    Game = game7,
    User = user3,
    Status = GameStatus.Planned,
    DateAdded = new DateTime(2023, 11, 1),
    HoursPlayed = 0,
    Review = null
};

var entry8 = new GameEntry
{
    Id = 8,
    Game = game3,
    User = user2,
    Status = GameStatus.Finished,
    DateAdded = new DateTime(2020, 10, 20),
    HoursPlayed = 95,
    Review = null
};

user1.GameEntries.AddRange(new[] { entry1, entry3 });
user2.GameEntries.AddRange(new[] { entry2, entry5, entry8 });
user3.GameEntries.AddRange(new[] { entry4, entry6, entry7 });

// ============= KOLEKCIJE ZA UPITE =============
var developers = new List<Developer> { developer1, developer2, developer3 };
var games = new List<Game> { game1, game2, game3, game4, game5, game6, game7, game8, game9 };
var users = new List<User> { user1, user2, user3 };
var gameEntries = new List<GameEntry> { entry1, entry2, entry3, entry4, entry5, entry6, entry7, entry8 };
var reviews = new List<Review> { review1, review2, review3, review4, review5, review6 };
var genres = new List<Genre> { genreRPG, genreAction, genreFPS, genreAdventure };
var platforms = new List<Platform> { platformPC, platformPS5, platformXbox };

// ============= LINQ UPITI =============
Console.WriteLine("==================== LINQ UPITI ====================\n");

// UPIT 1: Sve igre od CD Projekt Red developera sortirane po godini izdanja
Console.WriteLine("1. Igre CD Projekt Red (sortirane po godini):");
var cdProjektGames = games
    .Where(g => g.Developer?.Name == "CD Projekt Red")
    .OrderBy(g => g.ReleaseYear)
    .Select(g => new { g.Title, g.ReleaseYear });

foreach (var game in cdProjektGames)
{
    Console.WriteLine($"   - {game.Title} ({game.ReleaseYear})");
}
Console.WriteLine();

// UPIT 2: Igre sa prosječnom ocjenom > 90
Console.WriteLine("2. Igre sa prosječnom ocjenom > 90:");
var topRatedGames = games
    .Where(g => g.AverageRating > 90)
    .OrderByDescending(g => g.AverageRating);

foreach (var game in topRatedGames)
{
    Console.WriteLine($"   - {game.Title}: {game.AverageRating}/100");
}
Console.WriteLine();

// UPIT 3: Igre dostupne na PC
Console.WriteLine("3. Igre dostupne na PC:");
var pcGames = games
    .Where(g => g.Platforms.Any(p => p.Type == PlatformType.PC))
    .Select(g => g.Title)
    .ToList();

foreach (var game in pcGames)
{
    Console.WriteLine($"   - {game}");
}
Console.WriteLine();

// UPIT 4: Broj igara po developeru
Console.WriteLine("4. Broj igara po developeru:");
var gamesByDeveloper = developers
    .Select(d => new
    {
        Developer = d.Name,
        GameCount = d.Games.Count()
    })
    .OrderByDescending(d => d.GameCount);

foreach (var dev in gamesByDeveloper)
{
    Console.WriteLine($"   - {dev.Developer}: {dev.GameCount} igre");
}
Console.WriteLine();

// UPIT 5: Korisnici sortirani po broju završenih igara
Console.WriteLine("5. Korisnici sortirani po broju ZAVRŠENIH igara:");
var usersByFinished = users
    .Select(u => new
    {
        Username = u.Username,
        FinishedCount = u.GameEntries.Count(e => e.Status == GameStatus.Finished)
    })
    .OrderByDescending(u => u.FinishedCount);

foreach (var user in usersByFinished)
{
    Console.WriteLine($"   - {user.Username}: {user.FinishedCount} završenih igara");
}
Console.WriteLine();

// UPIT 6: Igre sa recenzijama (kompleks upit)
Console.WriteLine("6. Igre sa recenzijama:");
var withReviews = games
    .Where(g => g.Reviews.Count() > 0)
    .Select(g => new
    {
        g.Title,
        ReviewCount = g.Reviews.Count(),
        AverageScore = g.Reviews.Average(r => r.Score)
    });

foreach (var game in withReviews)
{
    Console.WriteLine($"   - {game.Title}: {game.ReviewCount} recenzije, prosječna ocjena: {game.AverageScore:F1}");
}
Console.WriteLine();

// UPIT 7: Sve recenzije korisnika "GamerPro" sa ocjenom >= 90
Console.WriteLine("7. Recenzije 'GamerPro' korisnika sa ocjenom >= 90:");
var gamerProReviews = reviews
    .Where(r => r.User?.Username == "GamerPro" && r.Score >= 90)
    .Select(r => new
    {
        Game = r.Game?.Title ?? "Unknown",
        r.Score,
        r.Comment
    });

foreach (var review in gamerProReviews)
{
    Console.WriteLine($"   - {review.Game}: {review.Score}/100 - \"{review.Comment}\"");
}
Console.WriteLine();

// UPIT 8: Developeri sa više od 2 igre
Console.WriteLine("8. Developeri sa više od 2 igre:");
var prolificDevelopers = developers
    .Where(d => d.Games.Count() > 2)
    .Select(d => new { d.Name, GameCount = d.Games.Count() });

foreach (var dev in prolificDevelopers)
{
    Console.WriteLine($"   - {dev.Name}: {dev.GameCount} igre");
}
Console.WriteLine();

// UPIT 9: Igre izdane nakon 2015. sortirane po ocjeni (LINQ sa First/FirstOrDefault)
Console.WriteLine("9. Najnovija igra (nakon 2015) sa najvećom ocjenom:");
var newestTopGame = games
    .Where(g => g.ReleaseYear > 2015)
    .OrderByDescending(g => g.AverageRating)
    .FirstOrDefault();

if (newestTopGame != null)
{
    Console.WriteLine($"   - {newestTopGame.Title} ({newestTopGame.ReleaseYear}): {newestTopGame.AverageRating}/100");
}
Console.WriteLine();

// UPIT 10: Žanrovi sortirani po broju igara
Console.WriteLine("10. Žanrovi sortirani po broju igara:");
var genresByCount = genres
    .Select(g => new
    {
        g.Name,
        GameCount = games.Count(game => game.Genres.Contains(g))
    })
    .OrderByDescending(g => g.GameCount);

foreach (var genre in genresByCount)
{
    Console.WriteLine($"   - {genre.Name}: {genre.GameCount} igre");
}
Console.WriteLine();

// UPIT 11: Ukupno sati utrošenih na igre po korisniku
Console.WriteLine("11. Ukupno sati igranja po korisniku:");
var hoursPerUser = users
    .Select(u => new
    {
        u.Username,
        TotalHours = u.GameEntries.Sum(e => e.HoursPlayed)
    })
    .OrderByDescending(u => u.TotalHours);

foreach (var user in hoursPerUser)
{
    Console.WriteLine($"   - {user.Username}: {user.TotalHours} sati");
}
Console.WriteLine();

// UPIT 12: Igre dostupne na više od 2 platforme
Console.WriteLine("12. Igre dostupne na više od 2 platforme:");
var multiPlatformGames = games
    .Where(g => g.Platforms.Count() > 2)
    .OrderByDescending(g => g.Platforms.Count());

foreach (var game in multiPlatformGames)
{
    Console.WriteLine($"   - {game.Title}: dostupna na {game.Platforms.Count()} platformi");
}
Console.WriteLine();

// UPIT 13: Igre sa žanrom "Action"
Console.WriteLine("13. Igre sa žanrom 'Action':");
var actionGames = games
    .Where(g => g.Genres.Any(gen => gen.Name == "Action"))
    .OrderByDescending(g => g.AverageRating);

foreach (var game in actionGames)
{
    Console.WriteLine($"   - {game.Title} ({game.AverageRating}/100)");
}
Console.WriteLine();

// UPIT 14: Igre koje igra "ActionJunkie"
Console.WriteLine("14. Igre koje igra 'ActionJunkie':");
var actionJunkie = users.FirstOrDefault(u => u.Username == "ActionJunkie");
if (actionJunkie != null)
{
    var actionJunkieGames = actionJunkie.GameEntries.Select(e => e.Game?.Title ?? "Unknown");
    foreach (var game in actionJunkieGames)
    {
        Console.WriteLine($"   - {game}");
    }
}
Console.WriteLine();

// UPIT 15: Planirane igre (nisu još početne)
Console.WriteLine("15. Planirane igre (nisu još početne):");
var plannedGames = gameEntries
    .Where(e => e.Status == GameStatus.Planned)
    .Select(e => new
    {
        Game = e.Game?.Title ?? "Unknown",
        User = e.User?.Username ?? "Unknown",
        DateAdded = e.DateAdded.ToShortDateString()
    });

foreach (var entry in plannedGames)
{
    Console.WriteLine($"   - {entry.Game} ({entry.User}, dodano: {entry.DateAdded})");
}

Console.WriteLine("\n==================== KRAJ UPITA ====================");

// ============= WEB APLIKACIJA SETUP =============
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
