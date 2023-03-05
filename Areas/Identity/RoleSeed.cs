using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VacationTracker.Areas.Identity.Data;
using VacationTracker.Models.Constants;

namespace VacationTracker.Areas.Identity
{
    public interface IRoleSeed
    {
        Task SeedAsync(RoleManager<IdentityRole> roleManager);
        Task SeedSystemAdminAsync(IServiceProvider serviceProvider);
    }

    public class RoleSeed : IRoleSeed
    {
        public async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new List<string> { Constants.Admin, Constants.Manager, Constants.Approver, Constants.Employee };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public async Task SeedSystemAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            IdentityResult roleResult;

            // Create SystemAdmin role if it don't exist
            var systemAdminRole = await roleManager.RoleExistsAsync("SystemAdmin");
            if (!systemAdminRole)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole("SystemAdmin"));
            }

            // Create System Admin user if they don't exist and assign it to SystemAdmin role
            var systemAdminUser = await userManager.FindByEmailAsync("admin@admin.com");
            if (systemAdminUser == null)
            {
                systemAdminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@admin.com"
                };
                var result = await userManager.CreateAsync(systemAdminUser, "Admin01!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(systemAdminUser, "SystemAdmin");
                }
            }
        }
    }

}
