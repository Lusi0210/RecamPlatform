using System;
using Microsoft.AspNetCore.Identity;
using Remp.Remp.DataAccess;
using Remp.Remp.Models.Entities;

namespace Remp.Remp.API.Seeders;

public class RoleSeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = { "Admin", "Agent" };

        foreach (string roleName in roleNames)
        {
            bool roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    public static async Task SeedAdminAsync(UserManager<IdentityUser> userManager, RempDbContext dbContext)
    {
        string adminEmail = "admin@recam.com";
        string adminPassword = "Admin123!";

        IdentityUser? existingUser = await userManager.FindByEmailAsync(adminEmail);
        if (existingUser != null) return;

        IdentityUser adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail
        };

        IdentityResult result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");

            PhotographyCompany company = new PhotographyCompany
            {
                Id = adminUser.Id,
                PhotographyCompanyName = "Recam Photography"
            };

            await dbContext.PhotographyCompanies.AddAsync(company);
            await dbContext.SaveChangesAsync();
        }
    }
}
