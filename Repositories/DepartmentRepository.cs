using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationTracker.Data;
using VacationTracker.Interfaces;
using VacationTracker.Models;

namespace VacationTracker.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _db;
        public DepartmentRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Department> AddDepartmentAsync(Department department)
        {
            _db.Departments.Add(department);
            await _db.SaveChangesAsync();
            return department;
        }

        public async Task DeleteDepartmentAsync(Department department)
        {
            department.IsDeleted = true;
            _db.Attach(department);
            _db.Entry(department).Property(x => x.IsDeleted).IsModified = true;
            await _db.SaveChangesAsync();
        }

        // Details page
        public async Task<Department> GetDepartmentByIdAndCompanyIdAsync(int id, int companyId)
        {
            // System admin (CompanyId = -1) can access all departments
            if (companyId == -1)
            {
                return await _db.Departments
                    .Include(x => x.Company)
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            }

            return await _db.Departments
                .Include(x => x.Company)
                .FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == companyId && !x.IsDeleted);
        }

        // Index page
        public async Task<IEnumerable<Department>> GetDepartmentsByCompanyIdAsync(int companyId)
        {
            // System admin (CompanyId = -1) can access all departments
            if (companyId == -1)
            {
                return await _db.Departments
                    .Include(d => d.Company)
                    .Where(x => !x.IsDeleted)
                    .ToListAsync();
            }

            return await _db.Departments
                .Include(d => d.Company)
                .Where(x => x.CompanyId == companyId && !x.IsDeleted)
                .ToListAsync();
        }

        public async Task UpdateDepartmentAsync(Department department)
        {
            _db.Attach(department).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DepartementExistsAsync(int id)
        {
            return await _db.Departments.AnyAsync(d => d.Id == id);
        }

        // Helper methods for EmployeeController
        public async Task<Department> GetDepartmentByIdAsync(int id)
        {
            return await _db.Departments
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _db.Departments
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.DepartmentName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Department>> SearchDepartmentsByNameAsync(string name, int companyId)
        {
            // System admin (CompanyId = -1) can access all departments
            if (companyId == -1)
            {
                return await _db.Departments
                    .Where(x => !x.IsDeleted && (string.IsNullOrEmpty(name) || x.DepartmentName.Contains(name)))
                    .OrderBy(x => x.DepartmentName)
                    .ToListAsync();
            }

            return await _db.Departments
                .Where(x => x.CompanyId == companyId && !x.IsDeleted && (string.IsNullOrEmpty(name) || x.DepartmentName.Contains(name)))
                .OrderBy(x => x.DepartmentName)
                .ToListAsync();
        }
    }
}
