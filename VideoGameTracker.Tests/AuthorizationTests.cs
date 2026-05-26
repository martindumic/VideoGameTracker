using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VideoGameTracker.Data;
using VideoGameTracker.Dtos;
using VideoGameTracker.Models;

namespace VideoGameTracker.Tests;

public class AuthorizationTests
{
    [Fact]
    public async Task PublicGetEndpoints_ShouldAllowAnonymous()
    {
        var dbName = $"auth-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            TestDataSeeder.SeedDeveloper(db);
            TestDataSeeder.SeedGenre(db);
            TestDataSeeder.SeedPlatform(db);
            var developer = TestDataSeeder.SeedDeveloper(db, "Extra Studio");
            var genre = TestDataSeeder.SeedGenre(db, "Extra Genre");
            var platform = TestDataSeeder.SeedPlatform(db, "Extra Platform");
            TestDataSeeder.SeedGame(db, developer, new List<Genre> { genre }, new List<Platform> { platform });
        }

        (await client.GetAsync("/api/games")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await client.GetAsync("/api/developers")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await client.GetAsync("/api/genres")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await client.GetAsync("/api/platforms")).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AdminEndpoints_ShouldAllowAdmin()
    {
        var dbName = $"auth-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        int developerId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            developerId = TestDataSeeder.SeedDeveloper(db).Id;
        }

        var dto = new DeveloperUpdateDto { Id = developerId, Name = "Updated" };
        var response = await client.PutAsJsonAsync($"/api/developers/{developerId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
