using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationTracker.Data;
using VacationTracker.Interfaces;
using VacationTracker.Models;

namespace VacationTracker.Repositories
{
    public class GenderRepository : IGenderRepository
    {
        private readonly ApplicationDbContext _db;

        public GenderRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        // Index page
        public async Task<IEnumerable<Gender>> GetGendersByCompanyIdAsync(int companyId)
        {
            // System admin (CompanyId = -1) can access all genders
            if (companyId == -1)
            {
                return await _db.Genders
                    .Where(x => !x.IsDeleted)
                    .ToListAsync();
            }
            
            return await _db.Genders
                .Where(x => x.CompanyId == companyId && !x.IsDeleted)
                .ToListAsync();
        }

        // Details and Edit page
        public async Task<Gender> GetGenderByIdAndCompanyIdAsync(int id, Company company)
        {
            // System admin (CompanyId = -1) can access all genders
            if (company.Id == -1)
            {
                return await _db.Genders
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            }
            
            return await _db.Genders
                .FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == company.Id && !x.IsDeleted);
        }

        public async Task<Gender> AddGenderAsync(Gender gender)
        {
            _db.Genders.Add(gender);
            await _db.SaveChangesAsync();
            return gender;
        }

        public async Task UpdateGenderAsync(Gender gender)
        {
            _db.Attach(gender).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteGenderAsync(Gender gender)
        {
            gender.IsDeleted = true;
            _db.Attach(gender);
            _db.Entry(gender).Property(x => x.IsDeleted).IsModified = true;
            await _db.SaveChangesAsync();
        }

        public async Task<bool> GenderExistsAsync(int id)
        {
            return await _db.Genders.AnyAsync(g => g.Id == id);
        }

        // Helper methods for EmployeeController
        public async Task<Gender> GetGenderByIdAsync(int id)
        {
            return await _db.Genders
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<IEnumerable<Gender>> GetAllGendersAsync()
        {
            return await _db.Genders
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Gender>> SearchGendersByNameAsync(string name, int companyId)
        {
            // System admin (CompanyId = -1) can access all genders
            if (companyId == -1)
            {
                return await _db.Genders
                    .Where(x => !x.IsDeleted && (string.IsNullOrEmpty(name) || x.Name.Contains(name)))
                    .OrderBy(x => x.Name)
                    .ToListAsync();
            }

            return await _db.Genders
                .Where(x => x.CompanyId == companyId && !x.IsDeleted && (string.IsNullOrEmpty(name) || x.Name.Contains(name)))
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}
