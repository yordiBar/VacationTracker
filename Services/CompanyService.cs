using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using VacationTracker.Areas.Identity.Extensions;
using VacationTracker.Data;
using VacationTracker.Interfaces;
using VacationTracker.Models;
using VacationTracker.SystemAdmin.Services.Interfaces;

namespace VacationTracker.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanySelectionService _companySelectionService;
        private readonly ICompanyDbContextFactory _dbContextFactory;
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository, ICompanySelectionService companySelectionService, ICompanyDbContextFactory dbContextFactory)
        {
            _companySelectionService = companySelectionService;
            _dbContextFactory = dbContextFactory;
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
            // If in SystemAdmin mode, use the selected company from session
            if (_companySelectionService.IsSystemAdminMode())
            {
                var selectedCompany = _companySelectionService.GetCurrentCompanyAsync().Result;
                return selectedCompany?.Id ?? 0;
            }

            // Otherwise, use the user's own company ID
            var userCompany = _companySelectionService.GetCurrentCompanyAsync().Result;
            return userCompany?.Id ?? 0;
        }

        public bool IsSystemAdmin()
        {
            // If in SystemAdmin mode but logged into a specific company, don't show SystemAdmin features
            if (_companySelectionService.IsSystemAdminMode())
            {
                return false;
            }

            var companyId = GetCurrentUserCompanyId();
            return companyId == -1; // System admin has CompanyId = -1
        }

        public ApplicationDbContext GetCurrentCompanyDbContext()
        {
            var company = _companySelectionService.GetCurrentCompanyAsync().Result;
            if (company == null)
                throw new InvalidOperationException("No company selected");

            return _dbContextFactory.CreateDbContext(company.Id);
        }

    }
}