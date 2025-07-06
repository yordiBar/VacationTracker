using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using VacationTracker.Areas.Identity.Extensions;
using VacationTracker.Interfaces;
using VacationTracker.Models;

namespace VacationTracker.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(IHttpContextAccessor httpContextAccessor, ICompanyRepository companyRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _companyRepository = companyRepository;
        }

        public async Task<Company> GetCurrentUserCompanyAsync()
        {
            var companyId = GetCurrentUserCompanyId();

            // System admin doesn't have a specific company
            if (IsSystemAdmin())
            {
                return null;
            }

            return await _companyRepository.GetCompanyByIdAsync(companyId);
        }

        public int GetCurrentUserCompanyId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                return user.Identity.GetCompanyId();
            }

            return 0; // Default value for unauthenticated users
        }

        public bool IsSystemAdmin()
        {
            var companyId = GetCurrentUserCompanyId();
            return companyId == -1; // System admin has CompanyId = -1
        }
    }
}