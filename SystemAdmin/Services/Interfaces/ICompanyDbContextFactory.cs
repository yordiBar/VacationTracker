using VacationTracker.Data;

namespace VacationTracker.SystemAdmin.Services.Interfaces
{
    public interface ICompanyDbContextFactory
    {
        ApplicationDbContext CreateDbContext(int companyId);
        ApplicationDbContext CreateDbContext(string databaseName);
    }
}