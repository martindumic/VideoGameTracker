using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VideoGameTracker.Data;
using VideoGameTracker.Dtos;
using VideoGameTracker.Models;

namespace VideoGameTracker.Tests;

public class PlatformsApiTests
{
    [Fact]
    public async Task GetAll_ShouldReturnPlatforms()
    {
        var dbName = $"platforms-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            TestDataSeeder.SeedPlatform(db, "PC");
        }

        var response = await client.GetAsync("/api/platforms");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<List<PlatformDto>>();
        dto.Should().NotBeNull();
        dto!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldFilterByQuery()
    {
        var dbName = $"platforms-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            TestDataSeeder.SeedPlatform(db, "ZetaStation");
            TestDataSeeder.SeedPlatform(db, "OmegaBox");
        }

        var response = await client.GetAsync("/api/platforms?query=Zeta");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<List<PlatformDto>>();
        dto.Should().NotBeNull();
        dto!.Should().ContainSingle(platform => platform.Name == "ZetaStation");
    }

    [Fact]
    public async Task GetAll_ShouldFilterByType()
    {
        var dbName = $"platforms-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            db.Platforms.Add(new Platform { Name = "PC", Type = PlatformType.PC });
            db.Platforms.Add(new Platform { Name = "Xbox", Type = PlatformType.Xbox });
            db.SaveChanges();
        }

        var response = await client.GetAsync("/api/platforms?type=PC");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<List<PlatformDto>>();
        dto.Should().NotBeNull();
        dto!.Should().OnlyContain(platform => platform.Type == PlatformType.PC.ToString());
    }

    [Fact]
    public async Task GetAll_ShouldReturnBadRequest_WhenTypeInvalid()
    {
        var dbName = $"platforms-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/platforms?type=NotAType");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_ShouldReturnPlatform_WhenExists()
    {
        var dbName = $"platforms-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        int platformId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            platformId = TestDataSeeder.SeedPlatform(db, "PC").Id;
        }

        var response = await client.GetAsync($"/api/platforms/{platformId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<PlatformDto>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(platformId);
        dto.Name.Should().Be("PC");
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenMissing()
    {
        var dbName = $"platforms-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/platforms/999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturnUnauthorized_WhenAnonymous()
    {
        var dbName = $"platforms-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        var dto = new PlatformCreateDto { Name = "New Platform", Type = PlatformType.PC };
        var response = await client.PostAsJsonAsync("/api/platforms", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_ShouldReturnForbidden_WhenNotAdmin()
    {
        var dbName = $"platforms-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("player-1", "player", "player@example.com", "Player");

        var dto = new PlatformCreateDto { Name = "New Platform", Type = PlatformType.PC };
        var response = await client.PostAsJsonAsync("/api/platforms", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Post_ShouldCreatePlatform_WhenValid()
    {
        var dbName = $"platforms-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var dto = new PlatformCreateDto { Name = "Created Platform", Type = PlatformType.PlayStation };
        var response = await client.PostAsJsonAsync("/api/platforms", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<PlatformDto>();
        created.Should().NotBeNull();
        created!.Name.Should().Be("Created Platform");
    }

    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenInvalid()
    {
        var dbName = $"platforms-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var dto = new PlatformCreateDto();
        var response = await client.PostAsJsonAsync("/api/platforms", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_ShouldUpdatePlatform_WhenValid()
    {
        var dbName = $"platforms-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        int platformId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            platformId = TestDataSeeder.SeedPlatform(db, "PC").Id;
        }

        var dto = new PlatformUpdateDto { Id = platformId, Name = "Updated Platform", Type = PlatformType.Xbox };
        var response = await client.PutAsJsonAsync($"/api/platforms/{platformId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<PlatformDto>();
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("Updated Platform");
    }

    [Fact]
    public async Task Put_ShouldReturnNotFound_WhenMissing()
    {
        var dbName = $"platforms-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var dto = new PlatformUpdateDto { Id = 999, Name = "Missing", Type = PlatformType.PC };
        var response = await client.PutAsJsonAsync("/api/platforms/999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_ShouldReturnBadRequest_WhenIdMismatch()
    {
        var dbName = $"platforms-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var dto = new PlatformUpdateDto { Id = 2, Name = "Mismatch", Type = PlatformType.PC };
        var response = await client.PutAsJsonAsync("/api/platforms/1", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ShouldRemovePlatform_WhenExists()
    {
        var dbName = $"platforms-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        int platformId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            platformId = TestDataSeeder.SeedPlatform(db, "PC").Id;
        }

        var response = await client.DeleteAsync($"/api/platforms/{platformId}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenMissing()
    {
        var dbName = $"platforms-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var response = await client.DeleteAsync("/api/platforms/999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
