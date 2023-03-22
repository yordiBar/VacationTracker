using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationTracker.Areas.Identity.Data;
using VacationTracker.Areas.Identity.Extensions;
using VacationTracker.Models;

namespace VacationTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {
        private readonly Data.ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        // Creating a connection to the database
        public EmployeeController(Data.ApplicationDbContext db,
            UserManager<ApplicationUser> userManager
            )
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();
            IEnumerable<Employee> employeeList = _db.Employees.Where(x => x.CompanyId == currentUsersCompanyId && x.IsDeleted == false);
            return View(employeeList);
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
                return View(employee);
            }
            int currentUsersCompanyId = User.Identity.GetCompanyId();
            employee.CompanyId = currentUsersCompanyId;
            _db.Employees.Add(employee);

            try
            {
                await _db.SaveChangesAsync();
                var user = new ApplicationUser { UserName = employee.Email, Email = employee.Email, CompanyId = employee.CompanyId };
                string password = employee.Password;
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
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

                    if (!await _userManager.IsInRoleAsync(user, "Employee"))
                    {
                        await _userManager.AddToRoleAsync(user, "Employee");
                    }
                }

                //check if there is an allowance for the current year for the employee if not create

                await CreateAllowanceIfRequired(employee, currentUsersCompanyId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CheckIfEmployeeExists(employee.Id))
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
            Employee employee = await _db.Employees.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId && x.IsDeleted == false);

            ApplicationUser newuser = await _userManager.FindByEmailAsync(employee.Email);
            employee.IsAdmin = await _userManager.IsInRoleAsync(newuser, "Admin");
            employee.IsApprover = await _userManager.IsInRoleAsync(newuser, "Approver");
            employee.IsManager = await _userManager.IsInRoleAsync(newuser, "Manager");

            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Employee employee)
        {

            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            employee.CompanyId = currentUsersCompanyId;

            _db.Attach(employee).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();

                ApplicationUser user = await _userManager.FindByEmailAsync(employee.Email);

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

                if (!await _userManager.IsInRoleAsync(user, "Employee"))
                {
                    await _userManager.AddToRoleAsync(user, "Employee");
                }
                // check if there is an allowance for the current year for the employee if not create
                await CreateAllowanceIfRequired(employee, currentUsersCompanyId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CheckIfEmployeeExists(employee.Id))
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

        // A boolean method to check if any employees exist
        private bool CheckIfEmployeeExists(int id)
        {
            return _db.Employees.Any(e => e.Id == id);
        }

        //check if there is an allowance for the current year for the employee if not create
        private async Task CreateAllowanceIfRequired(Employee emp, int currentUsersCompanyId)
        {
            if (_db.Allowances.Where(x => x.EmployeeId == emp.Id && x.CompanyId == currentUsersCompanyId).Count() == 0)
            {
                _db.Allowances.Add(new Allowance
                {
                    From = new DateTime(2020, 1, 1),
                    To = new DateTime(2020, 12, 31),
                    EmployeeId = emp.Id,
                    CompanyId = currentUsersCompanyId,
                    Amount = 20,
                    CarryOver = 0
                });

                _db.Allowances.Add(new Allowance
                {
                    From = new DateTime(2021, 1, 1),
                    To = new DateTime(2021, 12, 31),
                    EmployeeId = emp.Id,
                    CompanyId = currentUsersCompanyId,
                    Amount = 20,
                    CarryOver = 0
                });

                _db.Allowances.Add(new Allowance
                {
                    From = new DateTime(2022, 1, 1),
                    To = new DateTime(2022, 12, 31),
                    EmployeeId = emp.Id,
                    CompanyId = currentUsersCompanyId,
                    Amount = 20,
                    CarryOver = 0
                });

                await _db.SaveChangesAsync();
            };
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Employee emp = await _db.Employees.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == currentUsersCompanyId);

            if (emp == null)
            {
                return NotFound();
            }
            return View(emp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Employee emp)
        {
            if (!ModelState.IsValid)
            {
                return View(emp);
            }

            emp.IsDeleted = true;

            _db.Attach(emp).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CheckIfEmployeeExists(emp.Id))
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

        //********************************************************************************************************************
        //***************************************************HELPER METHODS***************************************************
        //********************************************************************************************************************

        [HttpGet]
        public IActionResult GetDepartmentById(int Id)
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Department dept = _db.Departments.Where(x => x.CompanyId == currentUsersCompanyId && x.IsDeleted == false && x.Id == Id).FirstOrDefault();

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
        public IActionResult GetDepartmentName(string name)
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();

            List<Department> departmentNameList = _db.Departments.Where(x => x.CompanyId == currentUsersCompanyId && x.IsDeleted == false).ToList();

            List<Department> departmentNameResults = new();

            foreach (var deptName in departmentNameList)
            {
                if (String.IsNullOrEmpty(name) || deptName.DepartmentName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    departmentNameResults.Add(deptName);
                }
            }

            departmentNameResults.Sort(delegate (Department d1, Department d2) { return d1.DepartmentName.CompareTo(d2.DepartmentName); });

            var serialisedJson = from result in departmentNameResults
                                 select new
                                 {
                                     text = result.DepartmentName,
                                     id = result.Id
                                 };
            return Json(serialisedJson);
        }

        [HttpGet]
        public IActionResult GetLocationById(int Id)
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Location location = _db.Locations.Where(x => x.CompanyId == currentUsersCompanyId && x.IsDeleted == false && x.Id == Id).FirstOrDefault();

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
        public IActionResult GetLocationName(string name)
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();

            List<Location> locationNameList = _db.Locations.Where(x => x.CompanyId == currentUsersCompanyId && x.IsDeleted == false).ToList();

            List<Location> locationNameResults = new();

            foreach (var locName in locationNameList)
            {
                if (String.IsNullOrEmpty(name) || locName.LocationName.Contains(name, StringComparison.OrdinalIgnoreCase))
                {
                    locationNameResults.Add(locName);
                }
            }

            locationNameResults.Sort(delegate (Location l1, Location l2) { return l1.LocationName.CompareTo(l2.LocationName); });

            var serialisedJson = from result in locationNameResults
                                 select new
                                 {
                                     text = result.LocationName,
                                     id = result.Id
                                 };
            return Json(serialisedJson);
        }

        [HttpGet]
        public IActionResult GetGenderById(int Id)
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();

            Gender gender = _db.Genders.Where(x => x.CompanyId == currentUsersCompanyId && x.IsDeleted == false && x.Id == Id).FirstOrDefault();

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
        public IActionResult GetGenderName(string name)
        {
            int currentUsersCompanyId = User.Identity.GetCompanyId();

            List<Gender> genderNameList = _db.Genders.Where(x => x.CompanyId == currentUsersCompanyId && x.IsDeleted == false).ToList();

            List<Gender> genderNameResults = new();

            foreach (var genName in genderNameList)
            {
                if (String.IsNullOrEmpty(name) || genName.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                {
                    genderNameResults.Add(genName);
                }
            }

            genderNameResults.Sort(delegate (Gender g1, Gender g2) { return g1.Name.CompareTo(g2.Name); });

            var serialisedJson = from result in genderNameResults
                                 select new
                                 {
                                     text = result.Name,
                                     id = result.Id
                                 };
            return Json(serialisedJson);
        }
    }
}
