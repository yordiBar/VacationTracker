using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VacationTracker.Areas.Identity
{
    public interface IRoleSeed
    {
        Task SeedAsync(RoleManager<IdentityRole> roleManager);
    }

    public class RoleSeed : IRoleSeed
    {
        public async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new List<string> { "Admin", "Manager", "Approver", "Employee" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }

}
