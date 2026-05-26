using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VideoGameTracker.Data;
using VideoGameTracker.Dtos;

namespace VideoGameTracker.Tests;

public class DevelopersApiTests
{
    [Fact]
    public async Task GetAll_ShouldReturnDevelopers()
    {
        var dbName = $"developers-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            TestDataSeeder.SeedDeveloper(db, "Alpha Dev");
        }

        var response = await client.GetAsync("/api/developers");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<List<DeveloperDto>>();
        dto.Should().NotBeNull();
        dto!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldFilterByQuery()
    {
        var dbName = $"developers-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            TestDataSeeder.SeedDeveloper(db, "Alpha Dev");
            TestDataSeeder.SeedDeveloper(db, "Beta Studio");
        }

        var response = await client.GetAsync("/api/developers?query=Beta");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<List<DeveloperDto>>();
        dto.Should().NotBeNull();
        dto!.Should().ContainSingle(dev => dev.Name == "Beta Studio");
    }

    [Fact]
    public async Task GetById_ShouldReturnDeveloper_WhenExists()
    {
        var dbName = $"developers-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        int developerId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            developerId = TestDataSeeder.SeedDeveloper(db, "Alpha Dev").Id;
        }

        var response = await client.GetAsync($"/api/developers/{developerId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<DeveloperDto>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(developerId);
        dto.Name.Should().Be("Alpha Dev");
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenMissing()
    {
        var dbName = $"developers-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/developers/999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturnUnauthorized_WhenAnonymous()
    {
        var dbName = $"developers-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        var dto = new DeveloperCreateDto { Name = "New Dev" };
        var response = await client.PostAsJsonAsync("/api/developers", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_ShouldReturnForbidden_WhenNotAdmin()
    {
        var dbName = $"developers-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("player-1", "player", "player@example.com", "Player");

        var dto = new DeveloperCreateDto { Name = "New Dev" };
        var response = await client.PostAsJsonAsync("/api/developers", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Post_ShouldCreateDeveloper_WhenValid()
    {
        var dbName = $"developers-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var dto = new DeveloperCreateDto { Name = "Created Dev", Country = "Test", Description = "Desc" };
        var response = await client.PostAsJsonAsync("/api/developers", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<DeveloperDto>();
        created.Should().NotBeNull();
        created!.Name.Should().Be("Created Dev");
    }

    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenInvalid()
    {
        var dbName = $"developers-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var dto = new DeveloperCreateDto();
        var response = await client.PostAsJsonAsync("/api/developers", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_ShouldUpdateDeveloper_WhenValid()
    {
        var dbName = $"developers-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        int developerId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            developerId = TestDataSeeder.SeedDeveloper(db, "Alpha Dev").Id;
        }

        var dto = new DeveloperUpdateDto { Id = developerId, Name = "Updated Dev" };
        var response = await client.PutAsJsonAsync($"/api/developers/{developerId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<DeveloperDto>();
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("Updated Dev");
    }

    [Fact]
    public async Task Put_ShouldReturnNotFound_WhenMissing()
    {
        var dbName = $"developers-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var dto = new DeveloperUpdateDto { Id = 999, Name = "Missing" };
        var response = await client.PutAsJsonAsync("/api/developers/999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_ShouldReturnBadRequest_WhenIdMismatch()
    {
        var dbName = $"developers-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var dto = new DeveloperUpdateDto { Id = 2, Name = "Mismatch" };
        var response = await client.PutAsJsonAsync("/api/developers/1", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ShouldRemoveDeveloper_WhenExists()
    {
        var dbName = $"developers-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        int developerId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            developerId = TestDataSeeder.SeedDeveloper(db, "Alpha Dev").Id;
        }

        var response = await client.DeleteAsync($"/api/developers/{developerId}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenMissing()
    {
        var dbName = $"developers-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var response = await client.DeleteAsync("/api/developers/999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
