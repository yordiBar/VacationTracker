using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationTracker.Data;

namespace VacationTracker.Models.Repositories
{
    public class GenderRepository : IGenderRepository
    {
        private readonly ApplicationDbContext _db;
        public GenderRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<Gender> AddGenderAsync(Gender gender)
        {
            _db.Genders.Add(gender);
            await _db.SaveChangesAsync();
            return gender;
        }

        public async Task DeleteGenderAsync(Gender gender)
        {
            gender.IsDeleted = true;
            _db.Attach(gender);
            _db.Entry(gender).Property(x => x.IsDeleted).IsModified = true;
            await _db.SaveChangesAsync();
        }

        public async Task<Gender> GetGenderByIdAndCompanyIdAsync(int id, int companyId)
        {
            return await _db.Genders
                .FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == companyId && !x.IsDeleted);
        }

        public async Task<IEnumerable<Gender>> GetGendersByCompanyIdAsync(int companyId)
        {
            return await _db.Genders
                .Where(x => x.CompanyId == companyId && !x.IsDeleted)
                .ToListAsync();
        }

        public async Task UpdateGenderAsync(Gender gender)
        {
            _db.Attach(gender).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
        public async Task<bool> GenderExistsAsync(int id)
        {
            return await _db.Genders.AnyAsync(x => x.Id == id);
        }
    }
}
