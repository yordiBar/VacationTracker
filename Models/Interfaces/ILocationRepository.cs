using System.Collections.Generic;
using System.Threading.Tasks;

namespace VacationTracker.Models.Interfaces
{
    public interface ILocationRepository
    {
        Task<IEnumerable<Location>> GetLocationsByCompanyIdAsync(int companyId);
        Task<Location> GetLocationByIdAndCompanyIdAsync(int id, int companyId);
        Task<Location> AddLocationAsync(Location location);
        Task UpdateLocationAsync(Location location);
        Task DeleteLocationAsync(Location location);
        Task<bool> LocationExistsAsync(int id);
    }
}
