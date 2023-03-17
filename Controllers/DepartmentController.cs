using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationTracker.Areas.Identity.Data;
using VacationTracker.Areas.Identity.Extensions;
using VacationTracker.Data;
using VacationTracker.Models;
using VacationTracker.Models.Repositories;

namespace VacationTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        #region Contructors
        private readonly IDepartmentRepository _departmentRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Serilog.ILogger _logger = Log.ForContext<DepartmentController>();

        public DepartmentController(IDepartmentRepository departmentRepostitory,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _departmentRepository = departmentRepostitory;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        #endregion

        #region Actions

        // GET: Department/Details
        public async Task<IActionResult> Index()
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();
            IEnumerable<Department> departmentList = await _departmentRepository.GetDepartmentsByCompanyIdAsync(currentUsersCompanyId);
            return View(departmentList);
        }

        // POST: Department/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.Error("Details method called with a null ID");
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Department department = await _departmentRepository.GetDepartmentByIdAndCompanyIdAsync(id.Value, currentUsersCompanyId);

            if (department == null)
            {
                _logger.Error("Department not found with ID {DepartmentId}", id);
                return NotFound();
            }
            return View(department);
        }

        // GET: Department/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Department());
        }


        // HTTPPost method to Create a department from Create view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department dept)
        {
            if (!ModelState.IsValid)
            {
                _logger.Error("Invalid model state while creating department");
                return View(dept);
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            dept.CompanyId = currentUsersCompanyId;

            await _departmentRepository.AddDepartmentAsync(dept);
            _logger.Information("Department created with ID {DepartmentId}", dept.Id);

            return RedirectToAction("Index");
        }

        // GET: Department/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.Error("Edit method called with a null ID");
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Department department = await _departmentRepository.GetDepartmentByIdAndCompanyIdAsync(id.Value, currentUsersCompanyId);

            if (department == null)
            {
                _logger.Error("Department not found with ID {DepartmentId}", id);
                return NotFound();
            }
            return View(department);
        }

        // POST: Department/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Department department)
        {
            if (!ModelState.IsValid)
            {
                _logger.Error("Invalid model state while editing department with ID {DepartmentId}", department.Id);
                return View(department);
            }

            await _departmentRepository.UpdateDepartmentAsync(department);
            _logger.Information("Department updated with ID {DepartmentId}", department.Id);

            return RedirectToAction("Index");
        }

        // GET: Department/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.Error("Delete method called with a null ID");
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Department department = await _departmentRepository.GetDepartmentByIdAndCompanyIdAsync(id.Value, currentUsersCompanyId);

            if (department == null)
            {
                _logger.Error("Department not found with ID {DepartmentId}", id);
                return NotFound();
            }

            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Department department)
        {
            if (!ModelState.IsValid)
            {
                _logger.Error("Invalid model state while deleting department with ID {DepartmentId}", department.Id);
                return View(department);
            }

            await _departmentRepository.DeleteDepartmentAsync(department);
            _logger.Information("Department deleted with ID {DepartmentId}", department.Id);

            return RedirectToAction("Index");
        }
        #endregion
    }
}
