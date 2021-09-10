using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationTracker.Areas.Identity.Data;
using VacationTracker.Areas.Identity.Extensions;
using VacationTracker.Data;
using VacationTracker.Models;

namespace VacationTracker.Controllers
{
    public class LocationController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LocationController(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        
        public IActionResult Index()
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();
            IEnumerable<Location> locationList = _db.Locations.Where(x => x.CompanyId == currentUsersCompanyId && x.IsDeleted == false);
            return View(locationList);
        }

        // GET: Location/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Location location = await _db.Locations.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId && x.IsDeleted == false);

            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }

        // A boolean method to check if any locations exist
        private bool LocationExists(int id)
        {
            return _db.Locations.Any(l => l.Id == id);
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
                return View(loc);
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            loc.CompanyId = currentUsersCompanyId;

            _db.Locations.Add(loc);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(loc.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Location location = await _db.Locations.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId);

            if (location == null)
            {
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
                return View(loc);
            }

            _db.Attach(loc).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(loc.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Location location = await _db.Locations.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId);

            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }

        // HttpPost method to delete locations
        [HttpPost]
        public async Task<IActionResult> Delete(Location loc)
        {
            if (!ModelState.IsValid)
            {
                return View(loc);
            }

            loc.IsDeleted = true;

            _db.Attach(loc).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(loc.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Index");
        }
    }
}
