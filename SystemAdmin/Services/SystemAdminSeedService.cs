using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using VacationTracker.Areas.Identity.Data;
using VacationTracker.SystemAdmin.Data;
using VacationTracker.SystemAdmin.Services.Interfaces;

namespace VacationTracker.SystemAdmin.Services
{
    public class SystemAdminSeedService : ISystemAdminSeedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MasterDbContext _masterDbContext;
        private readonly ILogger<SystemAdminSeedService> _logger;

        public SystemAdminSeedService(
            IServiceProvider serviceProvider,
            MasterDbContext masterDbContext,
            ILogger<SystemAdminSeedService> logger)
        {
            _serviceProvider = serviceProvider;
            _masterDbContext = masterDbContext;
            _logger = logger;
        }

        public async Task SeedSystemAdminAsync()
        {
            try
            {
                _logger.LogInformation("Starting SystemAdmin seeding process...");

                // Check if system admin already exists
                if (await SystemAdminExistsAsync())
                {
                    _logger.LogInformation("System admin already exists, skipping seeding");
                    return;
                }

                _logger.LogInformation("System admin does not exist, proceeding with creation...");

                var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                _logger.LogInformation("Retrieved UserManager and RoleManager services");

                // Create SystemAdmin role if it doesn't exist
                if (!await roleManager.RoleExistsAsync("SystemAdmin"))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole("SystemAdmin"));
                    if (roleResult.Succeeded)
                    {
                        _logger.LogInformation("Created SystemAdmin role successfully");
                    }
                    else
                    {
                        _logger.LogError("Failed to create SystemAdmin role: {Errors}", 
                            string.Join(", ", roleResult.Errors));
                        return;
                    }
                }
                else
                {
                    _logger.LogInformation("SystemAdmin role already exists");
                }

                // Create system admin user
                var systemAdmin = new ApplicationUser
                {
                    UserName = "systemadmin@vacationtracker.com",
                    Email = "systemadmin@vacationtracker.com",
                    EmailConfirmed = true,
                    CompanyId = -1, // Special ID for system admin
                    IsActive = true
                };

                _logger.LogInformation("Attempting to create system admin user: {Email}", systemAdmin.Email);

                var result = await userManager.CreateAsync(systemAdmin, "SystemAdmin123!");

                if (result.Succeeded)
                {
                    _logger.LogInformation("System admin user created successfully, adding to role...");
                    
                    var roleResult = await userManager.AddToRoleAsync(systemAdmin, "SystemAdmin");
                    if (roleResult.Succeeded)
                    {
                        _logger.LogInformation("System admin user added to SystemAdmin role successfully");
                    }
                    else
                    {
                        _logger.LogError("Failed to add system admin to role: {Errors}", 
                            string.Join(", ", roleResult.Errors));
                    }
                }
                else
                {
                    _logger.LogError("Failed to create system admin user: {Errors}", 
                        string.Join(", ", result.Errors));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding system admin");
                throw;
            }
        }

        public async Task<bool> SystemAdminExistsAsync()
        {
            try
            {
                var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var systemAdmin = await userManager.FindByEmailAsync("systemadmin@vacationtracker.com");
                
                if (systemAdmin == null)
                {
                    _logger.LogInformation("System admin user not found");
                    return false;
                }

                var isInRole = await userManager.IsInRoleAsync(systemAdmin, "SystemAdmin");
                _logger.LogInformation("System admin user found, is in role: {IsInRole}", isInRole);
                
                return isInRole;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if system admin exists");
                return false;
            }
        }
    }
}