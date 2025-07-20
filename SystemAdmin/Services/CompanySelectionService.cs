using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VacationTracker.SystemAdmin.Data;
using VacationTracker.SystemAdmin.Models;
using VacationTracker.SystemAdmin.Services.Interfaces;

namespace VacationTracker.SystemAdmin.Services
{
    public class CompanySelectionService : ICompanySelectionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MasterDbContext _masterDbContext;

        public CompanySelectionService(IHttpContextAccessor httpContextAccessor, MasterDbContext masterDbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _masterDbContext = masterDbContext;
        }

        public async Task<Company> GetCurrentCompanyAsync()
        {
            var companyId = _httpContextAccessor.HttpContext?.Session.GetInt32("CurrentCompanyId");

            if (!companyId.HasValue)
                return null;

            return await _masterDbContext.Companies
                .FirstOrDefaultAsync(c => c.Id == companyId.Value && c.IsActive);
        }

        public bool IsSystemAdminMode()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("SystemAdminMode") == "true";
        }

        public async Task SetCurrentCompanyAsync(int companyId)
        {
            var company = await _masterDbContext.Companies
                .FirstOrDefaultAsync(c => c.Id == companyId && c.IsActive);

            if (company != null)
            {
                _httpContextAccessor.HttpContext?.Session.SetInt32("CurrentCompanyId", companyId);
            }
        }

        public async Task<bool> HasAccessToCompanyAsync(int companyId)
        {
            return await _masterDbContext.Companies
                .AnyAsync(c => c.Id == companyId && c.IsActive);
        }
    }
}
