using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Serilog;
using VacationTracker.Data;
using VacationTracker.Models;

namespace VacationTracker.Controllers
{
    public class AllowanceController : Controller
    {
        #region Constructors
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger = Log.ForContext<LocationController>();
        #endregion

        #region Fields
        public AllowanceController(ApplicationDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Actions
        // GET: Allowance
        public async Task<IActionResult> Index()
        {
              return View(await _context.Allowances.ToListAsync());
        }

        // GET: Allowance/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Allowances == null)
            {
                return NotFound();
            }

            var allowance = await _context.Allowances
                .FirstOrDefaultAsync(m => m.Id == id);
            if (allowance == null)
            {
                return NotFound();
            }

            return View(allowance);
        }

        // GET: Allowance/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Allowance/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,From,To,Amount,CarryOver,EmployeeId,CompanyId")] Allowance allowance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(allowance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(allowance);
        }

        // GET: Allowance/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Allowances == null)
            {
                return NotFound();
            }

            var allowance = await _context.Allowances.FindAsync(id);
            if (allowance == null)
            {
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
            if (id != allowance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(allowance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AllowanceExists(allowance.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(allowance);
        }

        // GET: Allowance/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Allowances == null)
            {
                return NotFound();
            }

            var allowance = await _context.Allowances
                .FirstOrDefaultAsync(m => m.Id == id);
            if (allowance == null)
            {
                return NotFound();
            }

            return View(allowance);
        }

        // POST: Allowance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Allowances == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Allowances'  is null.");
            }
            var allowance = await _context.Allowances.FindAsync(id);
            if (allowance != null)
            {
                _context.Allowances.Remove(allowance);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Helpers
        private bool AllowanceExists(int id)
        {
          return _context.Allowances.Any(e => e.Id == id);
        }
        #endregion
    }
}
