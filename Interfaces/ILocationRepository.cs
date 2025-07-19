using System.Collections.Generic;
using System.Threading.Tasks;
using VacationTracker.Models;

namespace VacationTracker.Interfaces
{
    public interface ILocationRepository
    {
        Task<IEnumerable<Location>> GetLocationsByCompanyIdAsync(int companyId);
        Task<Location> GetLocationByIdAndCompanyIdAsync(int id, int companyId);
        Task<Location> AddLocationAsync(Location location);
        Task UpdateLocationAsync(Location location);
        Task DeleteLocationAsync(Location location);
        Task<bool> LocationExistsAsync(int id);
        Task<Location> GetLocationByIdAsync(int id);
        Task<IEnumerable<Location>> GetAllLocationsAsync();
        Task<IEnumerable<Location>> SearchLocationsByNameAsync(string name, int companyId);
    }
}
