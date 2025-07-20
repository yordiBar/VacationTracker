using System.Threading.Tasks;
using VacationTracker.SystemAdmin.Models;

namespace VacationTracker.SystemAdmin.Services.Interfaces
{
    public interface ICompanySelectionService
    {
        Task<Company> GetCurrentCompanyAsync();
        Task SetCurrentCompanyAsync(int companyId);
        Task<bool> HasAccessToCompanyAsync(int companyId);
        bool IsSystemAdminMode();
    }
}
