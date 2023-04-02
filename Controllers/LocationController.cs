using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;
using VacationTracker.Areas.Identity.Extensions;
using VacationTracker.Models;
using VacationTracker.Models.Repositories;

namespace VacationTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LocationController : Controller
    {
        #region Constructors
        private readonly ILocationRepository _locationRepository;
        private readonly ILogger _logger = Log.ForContext<LocationController>();
        #endregion

        #region Fields
        public LocationController(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }
        #endregion

        #region Actions

        public async Task<IActionResult> Index()
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();
            IEnumerable<Location> locationList = await _locationRepository.GetLocationsByCompanyIdAsync(currentUsersCompanyId);
            return View(locationList);
        }

        // GET: Location/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.Error("Details method called with a null ID");
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Location location = await _locationRepository.GetLocationByIdAndCompanyIdAsync(id.Value, currentUsersCompanyId);

            if (location == null)
            {
                _logger.Error("Location not found with ID {LocationId}", id);
                return NotFound();
            }
            return View(location);
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

            int currentUsersCompanyId = User.Identity.GetCompanyId();

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

            int currentUsersCompanyId = User.Identity.GetCompanyId();

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

            int currentUsersCompanyId = User.Identity.GetCompanyId();

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
