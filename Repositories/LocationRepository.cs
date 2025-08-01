﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationTracker.Data;
using VacationTracker.Interfaces;
using VacationTracker.Models;

namespace VacationTracker.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ApplicationDbContext _db;

        public LocationRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        // Index page
        public async Task<IEnumerable<Location>> GetLocationsByCompanyIdAsync(int companyId)
        {
            // System admin (CompanyId = -1) can access all locations
            if (companyId == -1)
            {
                return await _db.Locations
                    .Where(x => !x.IsDeleted)
                    .ToListAsync();
            }
            
            return await _db.Locations
                .Where(x => x.CompanyId == companyId && !x.IsDeleted)
                .ToListAsync();
        }

        // Details and Edit page
        public async Task<Location> GetLocationByIdAndCompanyIdAsync(int id, Company company)
        {
            // System admin (CompanyId = -1) can access all locations
            if (company.Id == -1)
            {
                return await _db.Locations
                    .Include(l => company)
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            }
            
            return await _db.Locations
                .Include(l => company)
                .FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == company.Id && !x.IsDeleted);
        }

        public async Task<Location> AddLocationAsync(Location location)
        {
            _db.Locations.Add(location);
            await _db.SaveChangesAsync();
            return location;
        }

        public async Task UpdateLocationAsync(Location location)
        {
            _db.Attach(location).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteLocationAsync(Location location)
        {
            location.IsDeleted = true;
            _db.Attach(location);
            _db.Entry(location).Property(x => x.IsDeleted).IsModified = true;
            await _db.SaveChangesAsync();
        }

        public async Task<bool> LocationExistsAsync(int id)
        {
            return await _db.Locations.AnyAsync(l => l.Id == id);
        }

        // Helper methods for EmployeeController
        public async Task<Location> GetLocationByIdAsync(int id)
        {
            return await _db.Locations
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<IEnumerable<Location>> GetAllLocationsAsync()
        {
            return await _db.Locations
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.LocationName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Location>> SearchLocationsByNameAsync(string name, int companyId)
        {
            // System admin (CompanyId = -1) can access all locations
            if (companyId == -1)
            {
                return await _db.Locations
                    .Where(x => !x.IsDeleted && (string.IsNullOrEmpty(name) || x.LocationName.Contains(name)))
                    .OrderBy(x => x.LocationName)
                    .ToListAsync();
            }

            return await _db.Locations
                .Where(x => x.CompanyId == companyId && !x.IsDeleted && (string.IsNullOrEmpty(name) || x.LocationName.Contains(name)))
                .OrderBy(x => x.LocationName)
                .ToListAsync();
        }
    }
}
