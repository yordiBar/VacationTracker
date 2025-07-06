using System.Collections.Generic;
using System.Threading.Tasks;
using VacationTracker.Models;

namespace VacationTracker.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company> GetCompanyByIdAsync(int id);
        Task<IEnumerable<Company>> GetAllCompaniesAsync();
        Task<Company> AddCompanyAsync(Company company);
        Task UpdateCompanyAsync(Company company);
        Task DeleteCompanyAsync(Company company);
        Task<bool> CompanyExistsAsync(int id);
        Task<Company> GetCompanyByNameAsync(string companyName);
    }
} 