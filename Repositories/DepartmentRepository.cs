using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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

        public async Task<Department> GetDepartmentByIdAndCompanyIdAsync(int id, int companyId)
        {
            return await _db.Departments
                .FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == companyId && !x.IsDeleted);
        }

        public async Task<IEnumerable<Department>> GetDepartmentsByCompanyIdAsync(int companyId)
        {
            return await _db.Departments
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
    }
}
