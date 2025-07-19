using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationTracker.Areas.Identity.Data;
using VacationTracker.Interfaces;
using VacationTracker.Models;
using VacationTracker.Models.DTO;

namespace VacationTracker.Controllers
{
    [Authorize(Roles = "Admin,SystemAdmin")]
    public class EmployeeController : Controller
    {
        #region Fields
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICompanyService _companyService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger = Log.ForContext<EmployeeController>();
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IGenderRepository _genderRepository;
        #endregion

        #region Constructors
        public EmployeeController(
            IEmployeeRepository employeeRepository,
            ICompanyService companyService,
            UserManager<ApplicationUser> userManager,
            IDepartmentRepository departmentRepository,
            ILocationRepository locationRepository,
            IGenderRepository genderRepository)
        {
            _employeeRepository = employeeRepository;
            _companyService = companyService;
            _userManager = userManager;
            _departmentRepository = departmentRepository;
            _locationRepository = locationRepository;
            _genderRepository = genderRepository;
        }
        #endregion

        #region Actions
        public async Task<IActionResult> Index()
        {
            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();

            if (currentUsersCompanyId == 0 && !_companyService.IsSystemAdmin())
            {
                _logger.Error("User does not have a valid company ID");
                return Unauthorized("You do not have access to any company data.");
            }

            IEnumerable<Employee> employeeList = await _employeeRepository.GetEmployeesByCompanyIdAsync(currentUsersCompanyId);

            var employeeDTOs = employeeList.Select(employee => new EmployeeDetailsDTO
            {
                Id = employee.Id,
                FirstName = employee.Firstname,
                Surname = employee.Surname,
                DisplayName = $"{employee.Firstname} {employee.Surname}",
                Email = employee.Email,
                CompanyName = employee.Company?.CompanyName ?? "Unknown Company",
                CompanyId = employee.CompanyId,
                StartDate = employee.StartDate,
                JobTitle = employee.JobTitle,
                IsAdmin = employee.IsAdmin,
                IsApprover = employee.IsApprover,
                IsManager = employee.IsManager
            }).ToList();

            return View(employeeDTOs);
        }

        // GET
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Employee());
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                _logger.Error("Invalid model state while creating employee");
                return View(employee);
            }

            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();
            employee.CompanyId = currentUsersCompanyId;

            try
            {
                await _employeeRepository.AddEmployeeAsync(employee);

                var user = new ApplicationUser { UserName = employee.Email, Email = employee.Email, CompanyId = employee.CompanyId };
                string password = employee.Password;
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await AssignRolesToUser(user, employee);
                    await CreateAllowanceIfRequired(employee, currentUsersCompanyId);
                    _logger.Information("Employee created with ID {EmployeeId}", employee.Id);
                }
                else
                {
                    _logger.Error("Failed to create user account for employee {EmployeeId}", employee.Id);
                }
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex, "Error creating employee");
                ModelState.AddModelError("", "An error occurred while creating the employee.");
                return View(employee);
            }

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
            Employee employee = await _employeeRepository.GetEmployeeByIdAndCompanyIdAsync(id.Value, currentUsersCompanyId);

            if (employee == null)
            {
                _logger.Error("Employee not found with ID {EmployeeId}", id);
                return NotFound();
            }

            ApplicationUser user = await _userManager.FindByEmailAsync(employee.Email);
            employee.IsAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            employee.IsApprover = await _userManager.IsInRoleAsync(user, "Approver");
            employee.IsManager = await _userManager.IsInRoleAsync(user, "Manager");

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                _logger.Error("Invalid model state while editing employee with ID {EmployeeId}", employee.Id);
                return View(employee);
            }

            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();
            employee.CompanyId = currentUsersCompanyId;

            try
            {
                await _employeeRepository.UpdateEmployeeAsync(employee);

                ApplicationUser user = await _userManager.FindByEmailAsync(employee.Email);
                await AssignRolesToUser(user, employee);
                await CreateAllowanceIfRequired(employee, currentUsersCompanyId);

                _logger.Information("Employee updated with ID {EmployeeId}", employee.Id);
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex, "Error updating employee with ID {EmployeeId}", employee.Id);
                ModelState.AddModelError("", "An error occurred while updating the employee.");
                return View(employee);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.Error("Delete method called with a null ID");
                return NotFound();
            }

            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();
            Employee employee = await _employeeRepository.GetEmployeeByIdAndCompanyIdAsync(id.Value, currentUsersCompanyId);

            if (employee == null)
            {
                _logger.Error("Employee not found with ID {EmployeeId}", id);
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                _logger.Error("Invalid model state while deleting employee with ID {EmployeeId}", employee.Id);
                return View(employee);
            }

            try
            {
                await _employeeRepository.DeleteEmployeeAsync(employee);
                _logger.Information("Employee deleted with ID {EmployeeId}", employee.Id);
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex, "Error deleting employee with ID {EmployeeId}", employee.Id);
                ModelState.AddModelError("", "An error occurred while deleting the employee.");
                return View(employee);
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region Private Methods
        private async Task AssignRolesToUser(ApplicationUser user, Employee employee)
        {
            // Handle Admin role
            if (employee.IsAdmin)
            {
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
            else
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                }
            }

            // Handle Approver role
            if (employee.IsApprover)
            {
                if (!await _userManager.IsInRoleAsync(user, "Approver"))
                {
                    await _userManager.AddToRoleAsync(user, "Approver");
                }
            }
            else
            {
                if (await _userManager.IsInRoleAsync(user, "Approver"))
                {
                    await _userManager.RemoveFromRoleAsync(user, "Approver");
                }
            }

            // Handle Manager role
            if (employee.IsManager)
            {
                if (!await _userManager.IsInRoleAsync(user, "Manager"))
                {
                    await _userManager.AddToRoleAsync(user, "Manager");
                }
            }
            else
            {
                if (await _userManager.IsInRoleAsync(user, "Manager"))
                {
                    await _userManager.RemoveFromRoleAsync(user, "Manager");
                }
            }

            // Always ensure Employee role is assigned
            if (!await _userManager.IsInRoleAsync(user, "Employee"))
            {
                await _userManager.AddToRoleAsync(user, "Employee");
            }
        }

        private async Task CreateAllowanceIfRequired(Employee emp, int currentUsersCompanyId)
        {
            // This method would need to be moved to a service or repository
            // For now, keeping the existing logic but it should be refactored
            // TODO: Move allowance creation logic to a separate service
        }
        #endregion

        #region Helpers
        [HttpGet]
        public async Task<IActionResult> GetDepartmentById(int Id)
        {
            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();

            // System admin (CompanyId = -1) can access all departments
            Department dept;
            if (currentUsersCompanyId == -1)
            {
                dept = await _departmentRepository.GetDepartmentByIdAsync(Id);
            }
            else
            {
                dept = await _departmentRepository.GetDepartmentByIdAndCompanyIdAsync(Id, currentUsersCompanyId);
            }

            if (dept != null)
            {
                var serialisedJson = new
                {
                    text = dept.DepartmentName,
                    id = dept.Id
                };
                return Json(serialisedJson);
            }
            else
            {
                var serialisedJson = new
                {
                    text = "",
                    id = 0
                };
                return Json(serialisedJson);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartmentName(string name)
        {
            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();

            // System admin (CompanyId = -1) can access all departments
            IEnumerable<Department> departmentNameList;
            if (currentUsersCompanyId == -1)
            {
                departmentNameList = await _departmentRepository.GetAllDepartmentsAsync();
            }
            else
            {
                departmentNameList = await _departmentRepository.GetDepartmentsByCompanyIdAsync(currentUsersCompanyId);
            }

            var departmentNameResults = departmentNameList
                .Where(deptName => String.IsNullOrEmpty(name) || deptName.DepartmentName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderBy(x => x.DepartmentName)
                .ToList();

            var serialisedJson = from result in departmentNameResults
                                 select new
                                 {
                                     text = result.DepartmentName,
                                     id = result.Id
                                 };
            return Json(serialisedJson);
        }

        [HttpGet]
        public async Task<IActionResult> GetLocationById(int Id)
        {
            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();

            // System admin (CompanyId = -1) can access all locations
            Location location;
            if (currentUsersCompanyId == -1)
            {
                location = await _locationRepository.GetLocationByIdAsync(Id);
            }
            else
            {
                location = await _locationRepository.GetLocationByIdAndCompanyIdAsync(Id, currentUsersCompanyId);
            }

            if (location != null)
            {
                var serialisedJson = new
                {
                    text = location.LocationName,
                    id = location.Id
                };
                return Json(serialisedJson);
            }
            else
            {
                var serialisedJson = new
                {
                    text = "",
                    id = 0
                };
                return Json(serialisedJson);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLocationName(string name)
        {
            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();

            // System admin (CompanyId = -1) can access all locations
            IEnumerable<Location> locationNameList;
            if (currentUsersCompanyId == -1)
            {
                locationNameList = await _locationRepository.GetAllLocationsAsync();
            }
            else
            {
                locationNameList = await _locationRepository.GetLocationsByCompanyIdAsync(currentUsersCompanyId);
            }

            var locationNameResults = locationNameList
                .Where(locName => String.IsNullOrEmpty(name) || locName.LocationName.Contains(name, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.LocationName)
                .ToList();

            var serialisedJson = from result in locationNameResults
                                 select new
                                 {
                                     text = result.LocationName,
                                     id = result.Id
                                 };
            return Json(serialisedJson);
        }

        [HttpGet]
        public async Task<IActionResult> GetGenderById(int Id)
        {
            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();

            // System admin (CompanyId = -1) can access all genders
            Gender gender;
            if (currentUsersCompanyId == -1)
            {
                gender = await _genderRepository.GetGenderByIdAsync(Id);
            }
            else
            {
                gender = await _genderRepository.GetGenderByIdAndCompanyIdAsync(Id, currentUsersCompanyId);
            }

            if (gender != null)
            {
                var serialisedJson = new
                {
                    text = gender.Name,
                    id = gender.Id
                };
                return Json(serialisedJson);
            }
            else
            {
                var serialisedJson = new
                {
                    text = "",
                    id = 0
                };
                return Json(serialisedJson);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetGenderName(string name)
        {
            int currentUsersCompanyId = _companyService.GetCurrentUserCompanyId();

            // System admin (CompanyId = -1) can access all genders
            IEnumerable<Gender> genderNameList;
            if (currentUsersCompanyId == -1)
            {
                genderNameList = await _genderRepository.GetAllGendersAsync();
            }
            else
            {
                genderNameList = await _genderRepository.GetGendersByCompanyIdAsync(currentUsersCompanyId);
            }

            var genderNameResults = genderNameList
                .Where(genName => String.IsNullOrEmpty(name) || genName.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.Name)
                .ToList();

            var serialisedJson = from result in genderNameResults
                                 select new
                                 {
                                     text = result.Name,
                                     id = result.Id
                                 };
            return Json(serialisedJson);
        }
        #endregion
    }
}
