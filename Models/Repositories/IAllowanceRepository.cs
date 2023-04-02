﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace VacationTracker.Models.Repositories
{
    public interface IAllowanceRepository
    {
        Task<IEnumerable<Allowance>> GetAllowancesByCompanyIdAsync(int companyId);
        Task<Allowance> GetAllowanceByIdAndCompanyIdAsync(int id, int companyId);
        Task<Allowance> AddAllowanceAsync(Allowance allowance);
        Task UpdateAllowanceAsync(Allowance allowance);
        //Task DeleteAllowanceAsync(Allowance allowance);
        Task<bool> AllowanceExistsAsync(int id);
    }
}
