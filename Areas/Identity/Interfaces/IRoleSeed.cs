using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;

namespace VacationTracker.Areas.Identity.Interfaces
{
    public interface IRoleSeed
    {
        Task SeedAsync(RoleManager<IdentityRole> roleManager);
        Task SeedSystemAdminAsync(IServiceProvider serviceProvider);
    }
}
