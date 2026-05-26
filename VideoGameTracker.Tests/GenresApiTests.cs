using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VideoGameTracker.Data;
using VideoGameTracker.Dtos;

namespace VideoGameTracker.Tests;

public class GenresApiTests
{
    [Fact]
    public async Task GetAll_ShouldReturnGenres()
    {
        var dbName = $"genres-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            TestDataSeeder.SeedGenre(db, "RPG");
        }

        var response = await client.GetAsync("/api/genres");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<List<GenreDto>>();
        dto.Should().NotBeNull();
        dto!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldFilterByQuery()
    {
        var dbName = $"genres-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            TestDataSeeder.SeedGenre(db, "Zeta Genre");
            TestDataSeeder.SeedGenre(db, "Omega Genre");
        }

        var response = await client.GetAsync("/api/genres?query=Zeta");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<List<GenreDto>>();
        dto.Should().NotBeNull();
        dto!.Should().ContainSingle(genre => genre.Name == "Zeta Genre");
    }

    [Fact]
    public async Task GetById_ShouldReturnGenre_WhenExists()
    {
        var dbName = $"genres-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        int genreId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            genreId = TestDataSeeder.SeedGenre(db, "RPG").Id;
        }

        var response = await client.GetAsync($"/api/genres/{genreId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<GenreDto>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(genreId);
        dto.Name.Should().Be("RPG");
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenMissing()
    {
        var dbName = $"genres-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/genres/999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturnUnauthorized_WhenAnonymous()
    {
        var dbName = $"genres-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        var dto = new GenreCreateDto { Name = "New Genre" };
        var response = await client.PostAsJsonAsync("/api/genres", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_ShouldReturnForbidden_WhenNotAdmin()
    {
        var dbName = $"genres-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("player-1", "player", "player@example.com", "Player");

        var dto = new GenreCreateDto { Name = "New Genre" };
        var response = await client.PostAsJsonAsync("/api/genres", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Post_ShouldCreateGenre_WhenValid()
    {
        var dbName = $"genres-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var dto = new GenreCreateDto { Name = "Created Genre", Description = "Desc" };
        var response = await client.PostAsJsonAsync("/api/genres", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<GenreDto>();
        created.Should().NotBeNull();
        created!.Name.Should().Be("Created Genre");
    }

    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenInvalid()
    {
        var dbName = $"genres-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var dto = new GenreCreateDto();
        var response = await client.PostAsJsonAsync("/api/genres", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_ShouldUpdateGenre_WhenValid()
    {
        var dbName = $"genres-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        int genreId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            genreId = TestDataSeeder.SeedGenre(db, "RPG").Id;
        }

        var dto = new GenreUpdateDto { Id = genreId, Name = "Updated Genre" };
        var response = await client.PutAsJsonAsync($"/api/genres/{genreId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<GenreDto>();
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("Updated Genre");
    }

    [Fact]
    public async Task Put_ShouldReturnNotFound_WhenMissing()
    {
        var dbName = $"genres-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var dto = new GenreUpdateDto { Id = 999, Name = "Missing" };
        var response = await client.PutAsJsonAsync("/api/genres/999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_ShouldReturnBadRequest_WhenIdMismatch()
    {
        var dbName = $"genres-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var dto = new GenreUpdateDto { Id = 2, Name = "Mismatch" };
        var response = await client.PutAsJsonAsync("/api/genres/1", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ShouldRemoveGenre_WhenExists()
    {
        var dbName = $"genres-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        int genreId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            genreId = TestDataSeeder.SeedGenre(db, "RPG").Id;
        }

        var response = await client.DeleteAsync($"/api/genres/{genreId}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenMissing()
    {
        var dbName = $"genres-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var response = await client.DeleteAsync("/api/genres/999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
