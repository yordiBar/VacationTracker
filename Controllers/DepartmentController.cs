using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationTracker.Areas.Identity.Extensions;
using VacationTracker.Interfaces;
using VacationTracker.Models;
using VacationTracker.Models.DTO;
using VacationTracker.Services;

namespace VacationTracker.Controllers
{
    [Authorize(Roles = "Admin,SystemAdmin")]
    public class DepartmentController : Controller
    {
        #region Fields
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ICompanyService _companyService;
        private readonly Serilog.ILogger _logger = Log.ForContext<DepartmentController>();
        #endregion

        #region Constructors
        public DepartmentController(IDepartmentRepository departmentRepostitory, ICompanyService companyService)
        {
            _departmentRepository = departmentRepostitory;
            _companyService = companyService;
        }
        #endregion

        #region Actions

        // GET: Department/Details
        public async Task<IActionResult> Index()
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();
            if (currentUsersCompanyId == 0 && !_companyService.IsSystemAdmin())
            {
                _logger.Error("User does not have a valid company ID");
                return Unauthorized("You do not have access to any company data.");
            }
            IEnumerable<Department> departmentList = await _departmentRepository.GetDepartmentsByCompanyIdAsync(currentUsersCompanyId);

            var departmentDTO = departmentList.Select(department => new DepartmentDetailsDTO
            {
                Id = department.Id,
                DepartmentName = department.DepartmentName,
                CompanyName = department.Company?.CompanyName ?? "Unknown Company",
                CompanyId = department.CompanyId
            }).ToList();

            return View(departmentDTO);
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

            var departmentDTO = new DepartmentDetailsDTO
            {
                Id = department.Id,
                DepartmentName = department.DepartmentName,
                CompanyName = department.Company?.CompanyName ?? "Unknown Company",
                CompanyId = department.CompanyId
            };
            return View(departmentDTO);
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
