using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using VacationTracker.Data;
using VacationTracker.SystemAdmin.Data;
using VacationTracker.SystemAdmin.Services.Interfaces;

namespace VacationTracker.SystemAdmin.Services
{
    public class CompanyDbContextFactory : ICompanyDbContextFactory
    {
        private readonly IConfiguration _configuration;
        private readonly MasterDbContext _masterDbContext;

        public CompanyDbContextFactory(IConfiguration configuration, MasterDbContext masterDbContext)
        {
            _configuration = configuration;
            _masterDbContext = masterDbContext;
        }

        public ApplicationDbContext CreateDbContext(int companyId)
        {
            var company = _masterDbContext.Companies
                .FirstOrDefault(c => c.Id == companyId && c.IsActive);

            if (company == null)
                throw new InvalidOperationException($"Company with ID {companyId} not found or inactive");

            return CreateDbContext(company.DatabaseName);
        }

        public ApplicationDbContext CreateDbContext(string databaseName)
        {
            var connectionString = _masterDbContext.Companies
                .Where(c => c.DatabaseName == databaseName && c.IsActive)
                .Select(c => c.ConnectionString)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException($"Database {databaseName} not found or inactive");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
