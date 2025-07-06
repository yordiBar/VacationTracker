using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;
using VacationTracker.Areas.Identity.Extensions;
using VacationTracker.Interfaces;
using VacationTracker.Models;

namespace VacationTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AllowanceController : Controller
    {
        #region Constructors
        private readonly IAllowanceRepository _allowanceRepository;
        private readonly ILogger _logger = Log.ForContext<LocationController>();
        #endregion

        #region Fields
        public AllowanceController(IAllowanceRepository allowanceRepository)
        {
            _allowanceRepository = allowanceRepository;
        }
        #endregion

        #region Actions
        // GET: Allowance
        public async Task<IActionResult> Index()
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();
            IEnumerable<Allowance> allowances = await _allowanceRepository.GetAllowancesByCompanyIdAsync(currentUsersCompanyId);
            return View(allowances);
        }

        // GET: Allowance/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.Error("Details method called with a null ID");
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Allowance allowance = await _allowanceRepository.GetAllowanceByIdAndCompanyIdAsync(id.Value, currentUsersCompanyId);

            if (allowance == null)
            {
                _logger.Error("Allowance not found with ID {AllowanceId}", id);
                return NotFound();
            }

            return View(allowance);
        }

        // GET: Allowance/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Allowance());
        }

        // POST: Allowance/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,From,To,Amount,CarryOver,EmployeeId,CompanyId")] Allowance allowance)
        {
            if (!ModelState.IsValid)
            {
                _logger.Error("Invalid model state while creating allowance");
                return View(allowance);
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            allowance.CompanyId = currentUsersCompanyId;

            await _allowanceRepository.AddAllowanceAsync(allowance);
            _logger.Information("Allowance created with ID {AllowanceId}", allowance.Id);

            return RedirectToAction("Index");
        }

        // GET: Allowance/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.Error("Edit method called with a null ID");
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Allowance allowance = await _allowanceRepository.GetAllowanceByIdAndCompanyIdAsync(id.Value, currentUsersCompanyId);

            if (allowance == null)
            {
                _logger.Error("Allowance not found with ID {AllowanceId}", id);
                return NotFound();
            }
            return View(allowance);
        }

        // POST: Allowance/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,From,To,Amount,CarryOver,EmployeeId,CompanyId")] Allowance allowance)
        {
            if (!ModelState.IsValid)
            {
                _logger.Error("Invalid model state while editing department with ID {AllowanceId}", allowance.Id);
                return View(allowance);
            }

            await _allowanceRepository.UpdateAllowanceAsync(allowance);
            _logger.Information("Allowance updated with ID {AllowanceId}", allowance.Id);

            return RedirectToAction("Index");
        }

        // GET: Allowance/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.Error("Delete method called with a null ID");
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Allowance allowance = await _allowanceRepository.GetAllowanceByIdAndCompanyIdAsync(id.Value, currentUsersCompanyId);

            if (allowance == null)
            {
                _logger.Error("Allowance not found with ID {AllowanceId}", id);
                return NotFound();
            }

            return View(allowance);
        }

        //// POST: Allowance/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Delete(Allowance allowance)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        _logger.Error("Invalid model state while deleting allowance with ID {AllowanceId}", allowance.Id);
        //        return View(allowance);
        //    }

        //    await _allowanceRepository.DeleteAllowanceAsync(allowance);
        //    _logger.Information("Allowance deleted with ID {AllowanceId}", allowance.Id);

        //    return RedirectToAction("Index");
        //}
        #endregion
    }
}
