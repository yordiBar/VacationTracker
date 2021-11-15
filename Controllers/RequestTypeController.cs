using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationTracker.Areas.Identity.Data;
using VacationTracker.Areas.Identity.Extensions;
using VacationTracker.Models;

namespace VacationTracker.Controllers
{
    public class RequestTypeController : Controller
    {
        private readonly VacationTracker.Data.ApplicationDbContext _db;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public RequestTypeController(Data.ApplicationDbContext db, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        // GET: RequestTypeController
        public IActionResult Index()
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();
            IEnumerable<RequestType> requestTypeList = _db.RequestTypes.Where(x => x.CompanyId == currentUsersCompanyId && x.IsDeleted == false);
            return View(requestTypeList);
        }

        // GET: RequestTypeController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();
            RequestType requestType = await _db.RequestTypes.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId && x.IsDeleted == false);

            if (requestType == null)
            {
                return NotFound();
            }
            return View(requestType);
        }

        private bool CheckIfRequestTypeExists(int id)
        {
            return _db.RequestTypes.Any(rt => rt.Id == id);
        }

        // GET: RequestTypeController/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new RequestType());
        }

        // POST: RequestTypeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RequestType requestType)
        {
            if (!ModelState.IsValid)
            {
                return View(requestType);
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();
            requestType.CompanyId = currentUsersCompanyId;
            _db.RequestTypes.Add(requestType);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CheckIfRequestTypeExists(requestType.Id))
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

        // GET: RequestTypeController/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            RequestType requestType = await _db.RequestTypes.FirstOrDefaultAsync(rt => rt.Id == id && rt.CompanyId == currentUsersCompanyId);

            if (requestType == null)
            {
                return NotFound();
            }
            return View(requestType);
        }

        // POST: RequestTypeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RequestType requestType)
        {
            if (!ModelState.IsValid)
            {
                return View(requestType);
            }

            _db.Attach(requestType).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!CheckIfRequestTypeExists(requestType.Id))
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

        // GET: RequestTypeController/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            RequestType requestType = await _db.RequestTypes.FirstOrDefaultAsync(rt => rt.Id == id && rt.CompanyId == currentUsersCompanyId);

            if (requestType == null)
            {
                return NotFound();
            }
            return View(requestType);
        }

        // POST: RequestTypeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(RequestType requestType)
        {
            if (!ModelState.IsValid)
            {
                return View(requestType);
            }

            requestType.IsDeleted = true;
            _db.Attach(requestType).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!CheckIfRequestTypeExists(requestType.Id))
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
