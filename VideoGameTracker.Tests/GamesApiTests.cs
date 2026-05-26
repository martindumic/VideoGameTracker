using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VideoGameTracker.Dtos;
using VideoGameTracker.Data;
using VideoGameTracker.Models;

namespace VideoGameTracker.Tests;

public class GamesApiTests
{
    [Fact]
    public async Task GetAll_ShouldReturnGames()
    {
        var dbName = $"games-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            TestDataSeeder.SeedGameWithRelations(db);
        }

        var response = await client.GetAsync("/api/games");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<List<GameDto>>();
        dto.Should().NotBeNull();
        dto!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldFilterByQuery()
    {
        var dbName = $"games-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            var developer = TestDataSeeder.SeedDeveloper(db);
            var genre = TestDataSeeder.SeedGenre(db);
            var platform = TestDataSeeder.SeedPlatform(db);
            TestDataSeeder.SeedGame(db, developer, new List<Genre> { genre }, new List<Platform> { platform });
            TestDataSeeder.SeedGame(db, developer, new List<Genre> { genre }, new List<Platform> { platform })
                .Title = "Another Game";
            db.SaveChanges();
        }

        var response = await client.GetAsync("/api/games?query=Another");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<List<GameDto>>();
        dto.Should().NotBeNull();
        dto!.Should().ContainSingle(game => game.Title == "Another Game");
    }

    [Fact]
    public async Task GetById_ShouldReturnGame_WhenGameExists()
    {
        var dbName = $"games-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        int gameId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            gameId = TestDataSeeder.SeedGameWithRelations(db);
        }

        var response = await client.GetAsync($"/api/games/{gameId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<GameDto>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(gameId);
        dto.Title.Should().Be("Test Game");
        dto.Developer.Should().NotBeNull();
        dto.Developer!.Name.Should().Be("Test Studio");
        dto.Genres.Should().ContainSingle();
        dto.Genres[0].Name.Should().Be("Test Genre");
        dto.Platforms.Should().ContainSingle();
        dto.Platforms[0].Name.Should().Be("Test Platform");
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenGameDoesNotExist()
    {
        var dbName = $"games-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/games/999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturnUnauthorized_WhenAnonymous()
    {
        var dbName = $"games-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        var dto = new GameCreateDto { Title = "New Game", ReleaseYear = 2024, DeveloperId = 1 };
        var response = await client.PostAsJsonAsync("/api/games", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_ShouldReturnForbidden_WhenNotAdmin()
    {
        var dbName = $"games-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("player-1", "player", "player@example.com", "Player");

        var dto = new GameCreateDto { Title = "New Game", ReleaseYear = 2024, DeveloperId = 1 };
        var response = await client.PostAsJsonAsync("/api/games", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Post_ShouldCreateGame_WhenModelIsValid()
    {
        var dbName = $"games-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        int developerId;
        int genreId;
        int platformId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            developerId = TestDataSeeder.SeedDeveloper(db).Id;
            genreId = TestDataSeeder.SeedGenre(db).Id;
            platformId = TestDataSeeder.SeedPlatform(db).Id;
        }

        var dto = new GameCreateDto
        {
            Title = "Created Game",
            ReleaseYear = 2021,
            Description = "New description",
            DeveloperId = developerId,
            AverageRating = 88,
            GenreIds = new List<int> { genreId },
            PlatformIds = new List<int> { platformId }
        };

        var response = await client.PostAsJsonAsync("/api/games", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<GameDto>();
        created.Should().NotBeNull();
        created!.Title.Should().Be("Created Game");
        created.Genres.Should().ContainSingle();
        created.Platforms.Should().ContainSingle();
    }

    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenModelIsInvalid()
    {
        var dbName = $"games-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var dto = new GameCreateDto();
        var response = await client.PostAsJsonAsync("/api/games", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_ShouldUpdateGame_WhenModelIsValid()
    {
        var dbName = $"games-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        int gameId;
        int developerId;
        int genreId;
        int platformId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            var developer = TestDataSeeder.SeedDeveloper(db);
            var genre = TestDataSeeder.SeedGenre(db);
            var platform = TestDataSeeder.SeedPlatform(db);
            gameId = TestDataSeeder.SeedGame(db, developer, new List<Genre> { genre }, new List<Platform> { platform }).Id;
            developerId = developer.Id;
            genreId = genre.Id;
            platformId = platform.Id;
        }

        var dto = new GameUpdateDto
        {
            Id = gameId,
            Title = "Updated Game",
            ReleaseYear = 2022,
            Description = "Updated",
            DeveloperId = developerId,
            AverageRating = 90,
            GenreIds = new List<int> { genreId },
            PlatformIds = new List<int> { platformId }
        };

        var response = await client.PutAsJsonAsync($"/api/games/{gameId}", dto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await response.Content.ReadFromJsonAsync<GameDto>();
        updated.Should().NotBeNull();
        updated!.Title.Should().Be("Updated Game");
    }

    [Fact]
    public async Task Put_ShouldReturnNotFound_WhenGameDoesNotExist()
    {
        var dbName = $"games-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var dto = new GameUpdateDto { Id = 404, Title = "Missing", ReleaseYear = 2020, DeveloperId = 1 };
        var response = await client.PutAsJsonAsync("/api/games/404", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_ShouldReturnBadRequest_WhenIdMismatch()
    {
        var dbName = $"games-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var dto = new GameUpdateDto { Id = 2, Title = "Mismatch", ReleaseYear = 2020, DeveloperId = 1 };
        var response = await client.PutAsJsonAsync("/api/games/1", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ShouldRemoveGame_WhenGameExists()
    {
        var dbName = $"games-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        int gameId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            gameId = TestDataSeeder.SeedGameWithRelations(db);
        }

        var response = await client.DeleteAsync($"/api/games/{gameId}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenGameDoesNotExist()
    {
        var dbName = $"games-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var response = await client.DeleteAsync("/api/games/404");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
