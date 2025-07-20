using System.Threading.Tasks;

namespace VacationTracker.SystemAdmin.Services.Interfaces
{
    public interface ISystemAdminSeedService
    {
        Task SeedSystemAdminAsync();
        Task<bool> SystemAdminExistsAsync();
    }
}