using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationTracker.Data;
using VacationTracker.Interfaces;
using VacationTracker.Models;

namespace VacationTracker.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _db;

        public CompanyRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Company> GetCompanyByIdAsync(int id)
        {
            return await _db.Companies
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
        {
            return await _db.Companies
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<Company> AddCompanyAsync(Company company)
        {
            _db.Companies.Add(company);
            await _db.SaveChangesAsync();
            return company;
        }

        public async Task UpdateCompanyAsync(Company company)
        {
            _db.Attach(company).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteCompanyAsync(Company company)
        {
            company.IsDeleted = true;
            _db.Attach(company);
            _db.Entry(company).Property(x => x.IsDeleted).IsModified = true;
            await _db.SaveChangesAsync();
        }

        public async Task<bool> CompanyExistsAsync(int id)
        {
            return await _db.Companies.AnyAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<Company> GetCompanyByNameAsync(string companyName)
        {
            return await _db.Companies
                .FirstOrDefaultAsync(c => c.CompanyName == companyName && !c.IsDeleted);
        }
    }
} 