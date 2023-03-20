using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VacationTracker.Models.Repositories
{
    public interface IGenderRepository
    {
        Task<IEnumerable<Gender>> GetGendersByCompanyIdAsync(int companyId);
        Task<Gender> GetGenderByIdAndCompanyIdAsync(int id, int companyId);
        Task<Gender> AddGenderAsync(Gender gender);
        Task UpdateGenderAsync(Gender gender);
        Task DeleteGenderAsync(Gender gender);
        Task<bool> GenderExistsAsync(int id);
    }
}
