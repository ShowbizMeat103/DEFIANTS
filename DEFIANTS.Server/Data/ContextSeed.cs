using DEFIANTS.Server.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace DEFIANTS.Server.Data;

public static class ContextSeed
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        if (!await roleManager.RoleExistsAsync("Jugador"))
        {
            await roleManager.CreateAsync(new IdentityRole("Jugador"));
        }
    }
}
