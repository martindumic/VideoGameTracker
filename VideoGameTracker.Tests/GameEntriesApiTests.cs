using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VideoGameTracker.Data;
using VideoGameTracker.Dtos;
using VideoGameTracker.Models;

namespace VideoGameTracker.Tests;

public class GameEntriesApiTests
{
    [Fact]
    public async Task GetAll_ShouldReturnEntries()
    {
        var dbName = $"entries-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            var developer = TestDataSeeder.SeedDeveloper(db);
            var genre = TestDataSeeder.SeedGenre(db);
            var platform = TestDataSeeder.SeedPlatform(db);
            var game = TestDataSeeder.SeedGame(db, developer, new List<Genre> { genre }, new List<Platform> { platform });
            TestDataSeeder.SeedUser(db, "player-1", "player@example.com", "player");
            TestDataSeeder.SeedGameEntry(db, game.Id, "player-1", GameStatus.Playing);
        }

        var response = await client.GetAsync("/api/gameentries");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<List<GameEntryDto>>();
        dto.Should().NotBeNull();
        dto!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldFilterByQuery()
    {
        var dbName = $"entries-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            var developer = TestDataSeeder.SeedDeveloper(db);
            var genre = TestDataSeeder.SeedGenre(db);
            var platform = TestDataSeeder.SeedPlatform(db);
            var game = TestDataSeeder.SeedGame(db, developer, new List<Genre> { genre }, new List<Platform> { platform });
            TestDataSeeder.SeedUser(db, "player-1", "player@example.com", "player");
            TestDataSeeder.SeedGameEntry(db, game.Id, "player-1", GameStatus.Playing);
        }

        var response = await client.GetAsync("/api/gameentries?query=Test Game");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<List<GameEntryDto>>();
        dto.Should().NotBeNull();
        dto!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldReturnBadRequest_WhenStatusInvalid()
    {
        var dbName = $"entries-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/gameentries?status=Nope");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_ShouldReturnEntry_WhenExists()
    {
        var dbName = $"entries-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        int entryId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            var developer = TestDataSeeder.SeedDeveloper(db);
            var genre = TestDataSeeder.SeedGenre(db);
            var platform = TestDataSeeder.SeedPlatform(db);
            var game = TestDataSeeder.SeedGame(db, developer, new List<Genre> { genre }, new List<Platform> { platform });
            TestDataSeeder.SeedUser(db, "player-1", "player@example.com", "player");
            entryId = TestDataSeeder.SeedGameEntry(db, game.Id, "player-1", GameStatus.Playing).Id;
        }

        var response = await client.GetAsync($"/api/gameentries/{entryId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<GameEntryDto>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(entryId);
        dto.Game.Should().NotBeNull();
        dto.User.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenMissing()
    {
        var dbName = $"entries-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/gameentries/999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturnUnauthorized_WhenAnonymous()
    {
        var dbName = $"entries-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        var dto = new GameEntryCreateDto { GameId = 1, Status = GameStatus.Playing };
        var response = await client.PostAsJsonAsync("/api/gameentries", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_ShouldCreateEntry_WhenValid()
    {
        var dbName = $"entries-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("player-1", "player", "player@example.com", "Player");

        int gameId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            var developer = TestDataSeeder.SeedDeveloper(db);
            var genre = TestDataSeeder.SeedGenre(db);
            var platform = TestDataSeeder.SeedPlatform(db);
            gameId = TestDataSeeder.SeedGame(db, developer, new List<Genre> { genre }, new List<Platform> { platform }).Id;
            TestDataSeeder.SeedUser(db, "player-1", "player@example.com", "player");
        }

        var dto = new GameEntryCreateDto
        {
            GameId = gameId,
            Status = GameStatus.Playing,
            HoursPlayed = 10,
            ReviewScore = 80,
            ReviewComment = "Great"
        };

        var response = await client.PostAsJsonAsync("/api/gameentries", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<GameEntryDto>();
        created.Should().NotBeNull();
        created!.Game.Should().NotBeNull();
        created.User.Should().NotBeNull();
    }

    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenInvalid()
    {
        var dbName = $"entries-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("player-1", "player", "player@example.com", "Player");

        var dto = new GameEntryCreateDto();
        var response = await client.PostAsJsonAsync("/api/gameentries", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_ShouldReturnNotFound_WhenGameMissing()
    {
        var dbName = $"entries-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("player-1", "player", "player@example.com", "Player");

        var dto = new GameEntryCreateDto { GameId = 999, Status = GameStatus.Playing };
        var response = await client.PostAsJsonAsync("/api/gameentries", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_ShouldUpdateEntry_WhenOwner()
    {
        var dbName = $"entries-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("player-1", "player", "player@example.com", "Player");

        int entryId;
        int gameId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            var developer = TestDataSeeder.SeedDeveloper(db);
            var genre = TestDataSeeder.SeedGenre(db);
            var platform = TestDataSeeder.SeedPlatform(db);
            gameId = TestDataSeeder.SeedGame(db, developer, new List<Genre> { genre }, new List<Platform> { platform }).Id;
            TestDataSeeder.SeedUser(db, "player-1", "player@example.com", "player");
            entryId = TestDataSeeder.SeedGameEntry(db, gameId, "player-1", GameStatus.Playing).Id;
        }

        var dto = new GameEntryUpdateDto
        {
            Id = entryId,
            GameId = gameId,
            Status = GameStatus.Finished,
            HoursPlayed = 20,
            ReviewScore = 90,
            ReviewComment = "Updated"
        };

        var response = await client.PutAsJsonAsync($"/api/gameentries/{entryId}", dto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await response.Content.ReadFromJsonAsync<GameEntryDto>();
        updated.Should().NotBeNull();
        updated!.Status.Should().Be(GameStatus.Finished);
    }

    [Fact]
    public async Task Put_ShouldReturnForbidden_WhenNotOwner()
    {
        var dbName = $"entries-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("player-2", "player2", "player2@example.com", "Player");

        int entryId;
        int gameId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            var developer = TestDataSeeder.SeedDeveloper(db);
            var genre = TestDataSeeder.SeedGenre(db);
            var platform = TestDataSeeder.SeedPlatform(db);
            gameId = TestDataSeeder.SeedGame(db, developer, new List<Genre> { genre }, new List<Platform> { platform }).Id;
            TestDataSeeder.SeedUser(db, "player-1", "player@example.com", "player");
            entryId = TestDataSeeder.SeedGameEntry(db, gameId, "player-1", GameStatus.Playing).Id;
        }

        var dto = new GameEntryUpdateDto { Id = entryId, GameId = gameId, Status = GameStatus.Finished };
        var response = await client.PutAsJsonAsync($"/api/gameentries/{entryId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Put_ShouldReturnNotFound_WhenMissing()
    {
        var dbName = $"entries-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("player-1", "player", "player@example.com", "Player");

        var dto = new GameEntryUpdateDto { Id = 999, GameId = 1, Status = GameStatus.Playing };
        var response = await client.PutAsJsonAsync("/api/gameentries/999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_ShouldReturnBadRequest_WhenIdMismatch()
    {
        var dbName = $"entries-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("player-1", "player", "player@example.com", "Player");

        var dto = new GameEntryUpdateDto { Id = 2, GameId = 1, Status = GameStatus.Playing };
        var response = await client.PutAsJsonAsync("/api/gameentries/1", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ShouldRemoveEntry_WhenOwner()
    {
        var dbName = $"entries-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("player-1", "player", "player@example.com", "Player");

        int entryId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            var developer = TestDataSeeder.SeedDeveloper(db);
            var genre = TestDataSeeder.SeedGenre(db);
            var platform = TestDataSeeder.SeedPlatform(db);
            var game = TestDataSeeder.SeedGame(db, developer, new List<Genre> { genre }, new List<Platform> { platform });
            TestDataSeeder.SeedUser(db, "player-1", "player@example.com", "player");
            entryId = TestDataSeeder.SeedGameEntry(db, game.Id, "player-1", GameStatus.Playing).Id;
        }

        var response = await client.DeleteAsync($"/api/gameentries/{entryId}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ShouldReturnForbidden_WhenNotOwner()
    {
        var dbName = $"entries-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("player-2", "player2", "player2@example.com", "Player");

        int entryId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            var developer = TestDataSeeder.SeedDeveloper(db);
            var genre = TestDataSeeder.SeedGenre(db);
            var platform = TestDataSeeder.SeedPlatform(db);
            var game = TestDataSeeder.SeedGame(db, developer, new List<Genre> { genre }, new List<Platform> { platform });
            TestDataSeeder.SeedUser(db, "player-1", "player@example.com", "player");
            entryId = TestDataSeeder.SeedGameEntry(db, game.Id, "player-1", GameStatus.Playing).Id;
        }

        var response = await client.DeleteAsync($"/api/gameentries/{entryId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenMissing()
    {
        var dbName = $"entries-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("player-1", "player", "player@example.com", "Player");

        var response = await client.DeleteAsync("/api/gameentries/999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
