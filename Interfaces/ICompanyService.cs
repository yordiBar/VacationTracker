using System.Threading.Tasks;
using VacationTracker.Models;

namespace VacationTracker.Interfaces
{
    public interface ICompanyService
    {
        Task<Company> GetCurrentUserCompanyAsync();
        int GetCurrentUserCompanyId();
        bool IsSystemAdmin();
    }
} 