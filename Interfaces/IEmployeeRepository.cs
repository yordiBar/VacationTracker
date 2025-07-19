using System.Collections.Generic;
using System.Threading.Tasks;
using VacationTracker.Models;

namespace VacationTracker.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetEmployeesByCompanyIdAsync(int companyId);
        Task<Employee> GetEmployeeByIdAndCompanyIdAsync(int id, int companyId);
        Task<Employee> AddEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(Employee employee);
        Task<bool> EmployeeExistsAsync(int id);
        Task<IEnumerable<Employee>> GetAllEmployeesAsync(); // For system admin
    }
}