using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationTracker.Data;
using VacationTracker.Interfaces;
using VacationTracker.Models;

namespace VacationTracker.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _db;

        public EmployeeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        // Index page
        public async Task<IEnumerable<Employee>> GetEmployeesByCompanyIdAsync(int companyId)
        {
            // System admin (CompanyId = -1) can access all employees
            if (companyId == -1)
            {
                return await _db.Employees
                    .Include(e => e.Company)
                    .Where(x => !x.IsDeleted)
                    .ToListAsync();
            }
            
            return await _db.Employees
                .Include(e => e.Company)
                .Where(x => x.CompanyId == companyId && !x.IsDeleted)
                .ToListAsync();
        }

        // For system admin to get all employees
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _db.Employees
                .Include(e => e.Company)
                .Where(x => !x.IsDeleted)
                .ToListAsync();
        }

        // Details and Edit page
        public async Task<Employee> GetEmployeeByIdAndCompanyIdAsync(int id, int companyId)
        {
            // System admin (CompanyId = -1) can access all employees
            if (companyId == -1)
            {
                return await _db.Employees
                    .Include(e => e.Company)
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            }
            
            return await _db.Employees
                .Include(e => e.Company)
                .FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == companyId && !x.IsDeleted);
        }

        public async Task<Employee> AddEmployeeAsync(Employee employee)
        {
            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();
            return employee;
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _db.Attach(employee).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(Employee employee)
        {
            employee.IsDeleted = true;
            _db.Attach(employee);
            _db.Entry(employee).Property(x => x.IsDeleted).IsModified = true;
            await _db.SaveChangesAsync();
        }

        public async Task<bool> EmployeeExistsAsync(int id)
        {
            return await _db.Employees.AnyAsync(e => e.Id == id);
        }
    }
}
