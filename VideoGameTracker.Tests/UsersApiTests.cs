using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VideoGameTracker.Data;
using VideoGameTracker.Dtos;
using VideoGameTracker.Models;

namespace VideoGameTracker.Tests;

public class UsersApiTests
{
    [Fact]
    public async Task GetAll_ShouldReturnUnauthorized_WhenAnonymous()
    {
        var dbName = $"users-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/users");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_ShouldReturnUsers_WhenAdmin()
    {
        var dbName = $"users-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            TestDataSeeder.SeedUser(db, "user-1", "user1@example.com", "user1");
        }

        var response = await client.GetAsync("/api/users");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<List<UserSummaryDto>>();
        dto.Should().NotBeNull();
        dto!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldFilterByQuery_WhenAdmin()
    {
        var dbName = $"users-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            TestDataSeeder.SeedUser(db, "user-1", "alpha@example.com", "alpha");
            TestDataSeeder.SeedUser(db, "user-2", "beta@example.com", "beta");
        }

        var response = await client.GetAsync("/api/users?query=beta");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<List<UserSummaryDto>>();
        dto.Should().NotBeNull();
        dto!.Should().ContainSingle(user => user.Email == "beta@example.com");
    }

    [Fact]
    public async Task GetById_ShouldReturnUser_WhenExists()
    {
        var dbName = $"users-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        string userId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VideoGameTrackerDbContext>();
            userId = TestDataSeeder.SeedUser(db, "user-1", "user1@example.com", "user1").Id;
        }

        var response = await client.GetAsync($"/api/users/{userId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<UserDetailDto>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(userId);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenMissing()
    {
        var dbName = $"users-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var response = await client.GetAsync("/api/users/missing");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldCreateUser_WhenValid()
    {
        var dbName = $"users-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var dto = new UserCreateDto
        {
            Email = "newuser@example.com",
            Password = "Password123!",
            OIB = "12345678901",
            JMBG = "1234567890123"
        };

        var response = await client.PostAsJsonAsync("/api/users", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<UserDetailDto>();
        created.Should().NotBeNull();
        created!.Email.Should().Be("newuser@example.com");
    }

    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenInvalid()
    {
        var dbName = $"users-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var dto = new UserCreateDto();
        var response = await client.PostAsJsonAsync("/api/users", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_ShouldUpdateUser_WhenValid()
    {
        var dbName = $"users-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        string userId;
        using (var scope = factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var user = new AppUser
            {
                UserName = "user1",
                Email = "user1@example.com",
                OIB = "12345678901",
                JMBG = "1234567890123",
                RegisteredAt = DateTime.UtcNow
            };
            await userManager.CreateAsync(user, "Password123!");
            userId = user.Id;
        }

        var dto = new UserUpdateDto { Id = userId, UserName = "updated" };
        var response = await client.PutAsJsonAsync($"/api/users/{userId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<UserDetailDto>();
        updated.Should().NotBeNull();
        updated!.UserName.Should().Be("updated");
    }

    [Fact]
    public async Task Put_ShouldReturnNotFound_WhenMissing()
    {
        var dbName = $"users-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var dto = new UserUpdateDto { Id = "missing", UserName = "none" };
        var response = await client.PutAsJsonAsync("/api/users/missing", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_ShouldReturnBadRequest_WhenIdMismatch()
    {
        var dbName = $"users-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var dto = new UserUpdateDto { Id = "user-2", UserName = "mismatch" };
        var response = await client.PutAsJsonAsync("/api/users/user-1", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ShouldRemoveUser_WhenNoEntries()
    {
        var dbName = $"users-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        string userId;
        using (var scope = factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var user = new AppUser
            {
                UserName = "user1",
                Email = "user1@example.com",
                OIB = "12345678901",
                JMBG = "1234567890123",
                RegisteredAt = DateTime.UtcNow
            };
            await userManager.CreateAsync(user, "Password123!");
            userId = user.Id;
        }

        var response = await client.DeleteAsync($"/api/users/{userId}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenMissing()
    {
        var dbName = $"users-api-{Guid.NewGuid():N}";
        using var factory = new CustomWebApplicationFactory(dbName);
        using var client = factory.CreateClient();
        client.SetTestUser("admin-1", "admin", "admin@example.com", "Admin");

        var response = await client.DeleteAsync("/api/users/missing");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
