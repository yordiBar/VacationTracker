using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationTracker.Interfaces;
using VacationTracker.Models;
using VacationTracker.Models.DTO;

namespace VacationTracker.Controllers
{
    [Authorize(Roles = "Admin,SystemAdmin")]
    public class LocationController : Controller
    {
        #region Fields
        private readonly ILocationRepository _locationRepository;
        private readonly ICompanyService _companyService;
        private readonly ILogger _logger = Log.ForContext<LocationController>();
        #endregion

        #region Constructors
        public LocationController(ILocationRepository locationRepository, ICompanyService companyService)
        {
            _locationRepository = locationRepository;
            _companyService = companyService;
        }


        #endregion

        #region Actions

        [HttpGet]
        public IActionResult Test()
        {
            return Content("LocationController.Test() reached successfully! Dependencies: " + 
                          (_locationRepository != null ? "Repository OK" : "Repository NULL") + ", " +
                          (_companyService != null ? "Service OK" : "Service NULL"));
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();
                
                _logger.Information("Current user company ID: {CompanyId}", currentUsersCompanyId);
                
                if (currentUsersCompanyId == 0 && !_companyService.IsSystemAdmin())
                {
                    _logger.Error("User does not have a valid company ID");
                    return Unauthorized("You do not have access to any company data.");
                }

                _logger.Information("Fetching locations for company ID: {CompanyId}", currentUsersCompanyId);
                IEnumerable<Location> locationList = await _locationRepository.GetLocationsByCompanyIdAsync(currentUsersCompanyId);
            
                            var locationDTOs = locationList.Select(location => new LocationDetailsDTO
                {
                    Id = location.Id,
                    LocationName = location.LocationName,
                    CompanyName = location.Company?.CompanyName ?? "Unknown Company",
                    CompanyId = location.CompanyId
                }).ToList();
                
                _logger.Information("Returning {Count} locations", locationDTOs.Count);
                return View(locationDTOs);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in LocationController.Index for company ID: {CompanyId}", 
                    _companyService.GetCurrentUserCompanyId());
                throw;
            }
        }

        // GET: Location/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.Error("Details method called with a null ID");
                return NotFound();
            }

            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();

            Location location = await _locationRepository.GetLocationByIdAndCompanyIdAsync(id.Value, currentUsersCompanyId);

            if (location == null)
            {
                _logger.Error("Location not found with ID {LocationId}", id);
                return NotFound();
            }

            var locationView = new LocationDetailsDTO
            {
                Id = location.Id,
                LocationName = location.LocationName,
                CompanyName = location.Company?.CompanyName ?? "Unknown Company",
                CompanyId = location.CompanyId
            };

            return View(locationView);
        }

        // GET: Location/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Location());
        }

        // HttpPost method to create locations
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Location loc)
        {
            if (!ModelState.IsValid)
            {
                _logger.Error("Invalid model state while creating location");
                return View(loc);
            }

            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();

            loc.CompanyId = currentUsersCompanyId;

            await _locationRepository.AddLocationAsync(loc);
            _logger.Information("Location created with ID {LocationId}", loc.Id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.Error("Edit method called with a null ID");
                return NotFound();
            }

            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();

            Location location = await _locationRepository.GetLocationByIdAndCompanyIdAsync(id.Value, currentUsersCompanyId);

            if (location == null)
            {
                _logger.Error("Location not found with ID {LocationId}", id);
                return NotFound();
            }
            return View(location);
        }

        // HttpPost method to edit locations
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Location loc)
        {
            if (!ModelState.IsValid)
            {
                _logger.Error("Invalid model state while editing location with ID {LocationId}", loc.Id);
                return View(loc);
            }

            await _locationRepository.UpdateLocationAsync(loc);
            _logger.Information("Location updated with ID {LocationId}", loc.Id);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.Error("Delete method called with a null ID");
                return NotFound();
            }

            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();

            Location location = await _locationRepository.GetLocationByIdAndCompanyIdAsync(id.Value, currentUsersCompanyId);

            if (location == null)
            {
                _logger.Error("Location not found with ID {LocationId}", id);
                return NotFound();
            }
            return View(location);
        }

        // HttpPost method to delete locations
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Location loc)
        {
            if (!ModelState.IsValid)
            {
                _logger.Error("Invalid model state while deleting location with ID {LocationId}", loc.Id);
                return View(loc);
            }

            await _locationRepository.DeleteLocationAsync(loc);
            _logger.Information("Location deleted with ID {LocationId}", loc.Id);

            return RedirectToAction("Index");
        }

        #endregion
    }
}
