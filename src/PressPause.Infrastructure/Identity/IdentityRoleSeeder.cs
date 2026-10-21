using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace PressPause.Infrastructure.Identity;

public static class IdentityRoleSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var roleManager =
            scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        string[] roles = ["User", "Admin"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(
                    new ApplicationRole { Name = role });

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to create role '{role}': " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}