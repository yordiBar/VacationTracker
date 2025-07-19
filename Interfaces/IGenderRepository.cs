using System.Collections.Generic;
using System.Threading.Tasks;
using VacationTracker.Models;

namespace VacationTracker.Interfaces
{
    public interface IGenderRepository
    {
        Task<IEnumerable<Gender>> GetGendersByCompanyIdAsync(int companyId);
        Task<Gender> GetGenderByIdAndCompanyIdAsync(int id, int companyId);
        Task<Gender> AddGenderAsync(Gender gender);
        Task UpdateGenderAsync(Gender gender);
        Task DeleteGenderAsync(Gender gender);
        Task<bool> GenderExistsAsync(int id);
        Task<Gender> GetGenderByIdAsync(int id);
        Task<IEnumerable<Gender>> GetAllGendersAsync();
        Task<IEnumerable<Gender>> SearchGendersByNameAsync(string name, int companyId);
    }
}
