using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using VacationTracker.Areas.Identity.Extensions;
using VacationTracker.Models;

namespace VacationTracker.Controllers
{
    [Authorize(Roles = "Admin,SystemAdmin")]
    public class RequestTypeController : Controller
    {
        #region Fields
        private readonly Data.ApplicationDbContext _db;
        private readonly ILogger<RequestTypeController> _logger;
        #endregion

        #region Constructors
        public RequestTypeController(Data.ApplicationDbContext db, ILogger<RequestTypeController> logger)
        {
            _db = db;
            _logger = logger;
        }
        #endregion

        #region Actions
        // GET: RequestTypeController
        public IActionResult Index()
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();
            
            // System admin (CompanyId = -1) can access all request types
            IEnumerable<RequestType> requestTypeList;
            if (currentUsersCompanyId == -1)
            {
                requestTypeList = _db.RequestTypes.Where(x => x.IsDeleted == false);
            }
            else
            {
                requestTypeList = _db.RequestTypes.Where(x => x.CompanyId == currentUsersCompanyId && x.IsDeleted == false);
            }
            
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
            
            // System admin (CompanyId = -1) can access all request types
            RequestType requestType;
            if (currentUsersCompanyId == -1)
            {
                requestType = await _db.RequestTypes.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            }
            else
            {
                requestType = await _db.RequestTypes.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId && x.IsDeleted == false);
            }

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
                _logger.LogInformation($"RequestType with ID {requestType.Id} created successfully");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"DbUpdateConcurrencyException occurred while creating RequestType with ID: {requestType.Id}");

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
            
            // System admin (CompanyId = -1) can access all request types
            RequestType requestType;
            if (currentUsersCompanyId == -1)
            {
                requestType = await _db.RequestTypes.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            }
            else
            {
                requestType = await _db.RequestTypes.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId && x.IsDeleted == false);
            }

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

        // GET: RequestTypeController/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();
            
            // System admin (CompanyId = -1) can access all request types
            RequestType requestType;
            if (currentUsersCompanyId == -1)
            {
                requestType = await _db.RequestTypes.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            }
            else
            {
                requestType = await _db.RequestTypes.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId && x.IsDeleted == false);
            }

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
        #endregion
    }
}
