using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VacationTracker.Areas.Identity.Data;
using VacationTracker.Areas.Identity.Interfaces;
using VacationTracker.Models.Constants;

namespace VacationTracker.Areas.Identity
{
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
            var logger = serviceProvider.GetRequiredService<ILogger<RoleSeed>>();

            IdentityResult roleResult;

            var systemAdminRole = await roleManager.RoleExistsAsync("SystemAdmin");
            if (!systemAdminRole)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole("SystemAdmin"));
                if (!roleResult.Succeeded)
                {
                    logger.LogError("Failed to create SystemAdmin role: {Errors}", string.Join(", ", roleResult.Errors));
                    return;
                }
                logger.LogInformation("SystemAdmin role created successfully");
            }

            var systemAdminUser = await userManager.FindByEmailAsync("admin@admin.com");
            if (systemAdminUser == null)
            {
                systemAdminUser = new ApplicationUser
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                    CompanyId = -1, // reserved for system admin
                    ContactName = "System Administrator"
                };

                var result = await userManager.CreateAsync(systemAdminUser, "Admin01!");
                if (result.Succeeded)
                {
                    logger.LogInformation("System admin user created successfully with email: {Email}", systemAdminUser.Email);

                    var roleAssignmentResult = await userManager.AddToRoleAsync(systemAdminUser, "SystemAdmin");
                    if (roleAssignmentResult.Succeeded)
                    {
                        logger.LogInformation("System admin user assigned to SystemAdmin role successfully");
                    }
                    else
                    {
                        logger.LogError("Failed to assign SystemAdmin role to user: {Errors}",
                            string.Join(", ", roleAssignmentResult.Errors));
                    }
                }
                else
                {
                    logger.LogError("Failed to create system admin user: {Errors}",
                        string.Join(", ", result.Errors));
                }
            }
            else
            {
                logger.LogInformation("System admin user already exists with email: {Email}", systemAdminUser.Email);

                if (!await userManager.IsInRoleAsync(systemAdminUser, "SystemAdmin"))
                {
                    var roleAssignmentResult = await userManager.AddToRoleAsync(systemAdminUser, "SystemAdmin");
                    if (roleAssignmentResult.Succeeded)
                    {
                        logger.LogInformation("System admin user assigned to SystemAdmin role successfully");
                    }
                    else
                    {
                        logger.LogError("Failed to assign SystemAdmin role to existing user: {Errors}",
                            string.Join(", ", roleAssignmentResult.Errors));
                    }
                }
            }
        }
    }
}
