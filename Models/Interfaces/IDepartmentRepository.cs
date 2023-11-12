using System.Collections.Generic;
using System.Threading.Tasks;

namespace VacationTracker.Models.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetDepartmentsByCompanyIdAsync(int companyId);
        Task<Department> GetDepartmentByIdAndCompanyIdAsync(int id, int companyId);
        Task<Department> AddDepartmentAsync(Department department);
        Task UpdateDepartmentAsync(Department department);
        Task DeleteDepartmentAsync(Department department);
        Task<bool> DepartementExistsAsync(int id);
    }
}
