using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public static class IdentitySeed
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, IWebHostEnvironment environment)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        var config = serviceProvider.GetRequiredService<IConfiguration>();

        await EnsureRoleAsync(roleManager, "Admin");
        await EnsureRoleAsync(roleManager, "Player");

        if (!environment.IsDevelopment())
        {
            return;
        }

        var adminEmail = config["Seed:AdminEmail"];
        var adminPassword = config["Seed:AdminPassword"];
        var adminOib = config["Seed:AdminOIB"];
        var adminJmbg = config["Seed:AdminJMBG"];

        if (string.IsNullOrWhiteSpace(adminEmail) ||
            string.IsNullOrWhiteSpace(adminPassword) ||
            string.IsNullOrWhiteSpace(adminOib) ||
            string.IsNullOrWhiteSpace(adminJmbg))
        {
            return;
        }

        var existing = await userManager.FindByEmailAsync(adminEmail);
        if (existing != null)
        {
            await EnsureUserRoleAsync(userManager, existing, "Admin");
            return;
        }

        var user = new AppUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            OIB = adminOib,
            JMBG = adminJmbg
        };

        var result = await userManager.CreateAsync(user, adminPassword);
        if (result.Succeeded)
        {
            await EnsureUserRoleAsync(userManager, user, "Admin");
        }
    }

    private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    private static async Task EnsureUserRoleAsync(UserManager<AppUser> userManager, AppUser user, string roleName)
    {
        if (!await userManager.IsInRoleAsync(user, roleName))
        {
            await userManager.AddToRoleAsync(user, roleName);
        }
    }
}
