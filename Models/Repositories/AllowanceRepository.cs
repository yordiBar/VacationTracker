using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationTracker.Data;
using VacationTracker.Models.Interfaces;

namespace VacationTracker.Models.Repositories
{
    public class AllowanceRepository : IAllowanceRepository
    {
        private readonly ApplicationDbContext _db;
        public AllowanceRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<Allowance> AddAllowanceAsync(Allowance allowance)
        {
            _db.Allowances.Add(allowance);
            await _db.SaveChangesAsync();
            return allowance;
        }

        //public Task DeleteAllowanceAsync(Allowance allowance)
        //{
        //    allowance.IsDeleted = true;
        //    _db.Attach(allowance);
        //    _db.Entry(allowance).Property(x => x.IsDeleted).IsModified = true;
        //    await _db.SaveChangesAsync();
        //}

        public async Task<Allowance> GetAllowanceByIdAndCompanyIdAsync(int id, int companyId)
        {
            return await _db.Allowances
                .FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == companyId && !x.IsDeleted);
        }

        public async Task<IEnumerable<Allowance>> GetAllowancesByCompanyIdAsync(int companyId)
        {
            DateTime dateTime = DateTime.Now;
            return await _db.Allowances
                .Where(x => x.CompanyId == companyId && x.To.Year == dateTime.Year && !x.IsDeleted)
                .ToListAsync();
        }

        public async Task UpdateAllowanceAsync(Allowance allowance)
        {
            _db.Attach(allowance).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public Task<bool> AllowanceExistsAsync(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
